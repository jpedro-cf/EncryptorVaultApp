using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Dtos.Files;

public class GetFileRequest
{   
    [FromHeader(Name = "X-Share-Id")]
    public string? ShareId { get; set; }
}