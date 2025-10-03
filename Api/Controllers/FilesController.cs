using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Api.Dtos.Files;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Api.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController(FilesService filesService): ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload([FromBody] UploadFileRequest data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(data);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await filesService.Upload(Guid.Parse(userId!), data);

        return Ok(result);
    }
    [HttpPost("uploads/complete")]
    [Authorize]
    public async Task<IActionResult> CompleteUpload([FromBody] CompleteUploadRequest data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(data);
        }
        
        var result = await filesService.CompleteUpload(data);

        return Ok(result);
    }
}