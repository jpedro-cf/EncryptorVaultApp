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
    public async Task<IResult> Upload([FromBody] UploadFileRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await filesService.Upload(Guid.Parse(userId), data);
        
        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.Ok(result.Data);
    }
    [HttpPost("uploads/complete")]
    [Authorize]
    public async Task<IResult> CompleteUpload([FromBody] CompleteUploadRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await filesService.CompleteUpload(Guid.Parse(userId), data);
        
        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IResult> Delete([FromRoute] string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await filesService.DeleteFile(Guid.Parse(userId), Guid.Parse(id));
        
        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.NoContent();
    }
}