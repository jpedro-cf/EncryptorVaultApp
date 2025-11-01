using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EncryptionApp.Api.Dtos.Files;
using Microsoft.IdentityModel.Tokens;
using File = EncryptionApp.Api.Entities.File;

namespace EncryptionApp.Api.Infra.Storage;

public class AmazonS3
{

    public readonly string _bucketName;
    private readonly IHostEnvironment _env;
    private readonly AmazonS3Client _client;
    private readonly string? _endpoint;

    public AmazonS3(IHostEnvironment env)
    {
        _bucketName = Environment.GetEnvironmentVariable("AWS_BUCKET_NAME") ?? "";
        _env = env;
        _endpoint = Environment.GetEnvironmentVariable("AWSdp_ENDPOINT");
        
        var credentials = new BasicAWSCredentials(
            Environment.GetEnvironmentVariable("AWS_ACCESS") ?? "", 
            Environment.GetEnvironmentVariable("AWS_SECRET") ?? "");

        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_REGION")),
            ForcePathStyle = true,
            UseHttp = true
        };
        
        if (!_endpoint.IsNullOrEmpty())
        {
            config.ServiceURL = _endpoint;
        }
        
        _client = new AmazonS3Client(credentials, config);
    }

    public async Task<InitiateUploadResponse> InitiateMultiPartUpload(File file)
    {
        var initRequest = new InitiateMultipartUploadRequest
        {
            BucketName = _bucketName,
            Key = file.StorageKey,
        };
        
        initRequest.Metadata.Add("file_id", file.Id.ToString());
        
        var initResponse = await _client.InitiateMultipartUploadAsync(initRequest);
        var presignedUrls = new List<PresignedPartUrl>();
        
        const int chunkSizeMb = 100;
        int totalParts = (int)Math.Ceiling((double)file.Size / (chunkSizeMb * 1024 * 1024));
        
        for (var i = 1; i <= totalParts; i++)
        {
            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = file.StorageKey,
                Verb = HttpVerb.PUT,
                Protocol = _env.IsDevelopment() ? Protocol.HTTP : Protocol.HTTPS,
                PartNumber = i,
                UploadId = initResponse.UploadId,
                Expires = DateTime.UtcNow.AddMinutes(30),
                ContentType = "application/octet-stream",
            };

            var url = await _client.GetPreSignedURLAsync(urlRequest);
            presignedUrls.Add(new PresignedPartUrl(i, url));
        }
        
        return new InitiateUploadResponse(file.Id.ToString(), initResponse.UploadId, file.StorageKey, presignedUrls);
    }

    public async Task<UploadCompletedResponse> CompleteMultiPartUpload(CompleteUploadRequest data)
    {
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = _bucketName,
            Key = data.Key,
            UploadId = data.UploadId,
            PartETags = data.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };
        
        var response = await _client.CompleteMultipartUploadAsync(completeRequest);

        return new UploadCompletedResponse(response.Key);
    }

    public async Task<List<UploadPart>> ListUploadParts(string key, string uploadId)
    {
        var data = await _client.ListPartsAsync(_bucketName, key, uploadId);
        
        return data.Parts
            .Select(p => new UploadPart(p.LastModified, p.Size, p.PartNumber, p.ETag))
            .ToList();
    }

    public async Task AbortMultiPartUpload(string key, string uploadId)
    {
        await _client.AbortMultipartUploadAsync(_bucketName, key, uploadId);
    }

    public async Task<string> GeneratePresignedUrl(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Protocol = _env.IsDevelopment() ? Protocol.HTTP : Protocol.HTTPS,
            Expires = DateTime.UtcNow.AddHours(3),
            ContentType = "application/octet-stream",
        };

        return await _client.GetPreSignedURLAsync(request);
    }

    public async Task<GetObjectMetadataResponse> GetObjectMetadata(string key)
    {
        return await _client.GetObjectMetadataAsync(_bucketName, key);
    }

    public async Task DeleteObject(string key)
    {
        await _client.DeleteObjectAsync(_bucketName, key);
    }
}