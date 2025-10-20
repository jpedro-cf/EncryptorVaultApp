using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Dtos.Folders;

public class GetFolderRequest
{   
    [FromHeader(Name = "shareId")]
    public string? ShareId { get; set; }
    
}