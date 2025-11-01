using EncryptionApp.Api.Dtos.Validations;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Dtos.Files;

public class GetFileRequest
{   
    [FromHeader(Name = "X-Share-Id")]
    [Guid]
    public string? ShareId { get; set; }
}