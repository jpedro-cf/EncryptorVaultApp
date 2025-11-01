using EncryptionApp.Api.Dtos.Validations;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Dtos.Folders;

public class GetFolderRequest
{   
    [FromHeader(Name = "X-Share-Id")]
    [Guid]
    public string? ShareId { get; set; }
}