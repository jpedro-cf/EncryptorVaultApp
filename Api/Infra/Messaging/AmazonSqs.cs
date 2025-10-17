using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.IdentityModel.Tokens;
using MyMVCProject.Api.Infra.Storage;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Api.Infra.Messaging;

public class AmazonSqs : BackgroundService
{
    
    private readonly IAmazonSQS _client;
    private readonly string _queueName;
    private readonly AmazonS3 _s3;
    
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AmazonSqs> _logger;

    public AmazonSqs(
        IConfiguration configuration, 
        IHostEnvironment env, 
        AmazonS3 amazonS3,
        ILogger<AmazonSqs> logger, 
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _s3 = amazonS3;
        
        _queueName = configuration["AWS:QueueName"] ?? "";
        
        
        var credentials = new BasicAWSCredentials(configuration["AWS:AccessKey"] ?? "", configuration["AWS:SecretKey"] ?? "");
        
        var config = new AmazonSQSConfig
        {
             RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]),
        };
        
        if (env.IsDevelopment())
        {
            config.ServiceURL = "http://localhost:4566"; // LocalStack
            config.UseHttp = true;
        }

        _client = new AmazonSQSClient(credentials, config);
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var queueUrl = (await _client.GetQueueUrlAsync(_queueName, ct)).QueueUrl;
        if (queueUrl == null)
        {
            _logger.LogError("Queue url is null.");
            return;
        }
        
        var receiveRequest = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            WaitTimeSeconds = 20, // long polling
            MaxNumberOfMessages = 10
        };

        while (!ct.IsCancellationRequested)
        {
            var response = await _client.ReceiveMessageAsync(receiveRequest, ct);
            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                // TODO: Handle later
                _logger.LogError("Error receiving message from SQS.");
                continue;
            }

            if (response.Messages.IsNullOrEmpty())
            {
                continue;
            }
            
            foreach (var message in response.Messages)
            {
                try
                {
                    var s3Event = Amazon.S3.Util.S3EventNotification.ParseJson(message.Body);
                    
                    _logger.LogInformation("Received S3 Event and parsed the JSON.");

                    foreach (var record in s3Event.Records)
                    {
                        var metadata = await _s3.GetObjectMetadata(record.S3.Object.Key);
                        var fileId = metadata.Metadata["x-amz-meta-fileid"];
                        
                        if (fileId.IsNullOrEmpty())
                        {
                            continue;
                        }
                        
                        using var scope = _scopeFactory.CreateScope();
                        var filesService = scope.ServiceProvider.GetRequiredService<FilesService>();
                        
                        // confirm the upload with the correct file size
                        // in case a malicious user has sent the request with altered data
                        await filesService.ConfirmUpload(fileId, metadata.ContentLength);
                    }

                    await _client.DeleteMessageAsync(queueUrl, message.ReceiptHandle, ct);
                    _logger.LogInformation("S3 Event processed.");
                }
                catch (Exception e)
                {
                    _logger.LogError("Error processing S3 Event.");
                }
            }
        }
    }
}