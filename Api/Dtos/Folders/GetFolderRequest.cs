using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Dtos.Folders;

public class GetFolderRequest
{   
    [FromHeader(Name = "X-Share-Id")]
    public string? ShareId { get; set; }
}