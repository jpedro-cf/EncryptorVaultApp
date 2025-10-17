using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using MyMVCProject.Api.Dtos.Files;
using File = MyMVCProject.Api.Entities.File;

namespace MyMVCProject.Api.Infra.Storage;

public class AmazonS3
{
    public string BucketName { get; private set; }

    private readonly IHostEnvironment Env;
    private readonly AmazonS3Client _client;

    public AmazonS3(IConfiguration configuration, IHostEnvironment env)
    {
        BucketName = configuration["AWS:BucketName"] ?? "";
        Env = env;
        
        var credentials = new BasicAWSCredentials(configuration["AWS:AccessKey"] ?? "", configuration["AWS:SecretKey"] ?? "");

        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
        };
        
        if (env.IsDevelopment())
        {
            config.ServiceURL = "http://localhost:4566"; // LocalStack
            config.ForcePathStyle = true; // LocalStack
            config.UseHttp = true;
        }
        
        _client = new AmazonS3Client(credentials, config);
    }

    public async Task<InitiateUploadResponse> InitiateUpload(File file)
    {
        var initRequest = new InitiateMultipartUploadRequest
        {
            BucketName = BucketName,
            Key = file.StorageKey,
        };
        
        initRequest.Metadata.Add("fileId", file.Id.ToString());
        
        var initResponse = await _client.InitiateMultipartUploadAsync(initRequest);
        var presignedUrls = new List<PresignedPartUrl>();
        
        const int chunkSizeMb = 100;
        int totalParts = (int)Math.Ceiling((double)file.Size / (chunkSizeMb * 1024 * 1024));
        
        for (var i = 1; i <= totalParts; i++)
        {
            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = file.StorageKey,
                Verb = HttpVerb.PUT,
                Protocol = Env.IsDevelopment() ? Protocol.HTTP : Protocol.HTTPS,
                PartNumber = i,
                UploadId = initResponse.UploadId,
                Expires = DateTime.UtcNow.AddMinutes(30),
                ContentType = "application/octet-stream",
            };

            var url = await _client.GetPreSignedURLAsync(urlRequest);
            presignedUrls.Add(new PresignedPartUrl(i, url));
        }
        
        return new InitiateUploadResponse(initResponse.UploadId, file.StorageKey, presignedUrls);
    }

    public async Task<UploadCompletedResponse> CompletedUpload(CompleteUploadRequest data)
    {
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = BucketName,
            Key = data.Key,
            UploadId = data.UploadId,
            PartETags = data.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };
        
        var response = await _client.CompleteMultipartUploadAsync(completeRequest);

        return new UploadCompletedResponse(response.Key);
    }

    public async Task<GetObjectMetadataResponse> GetObjectMetadata(string key)
    {
        return await _client.GetObjectMetadataAsync(BucketName, key);
    }
}