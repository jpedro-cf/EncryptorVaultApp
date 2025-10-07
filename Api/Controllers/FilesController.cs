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
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await filesService.Upload(Guid.Parse(userId!), data);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        return Results.Ok(result.Data);
    }
    [HttpPost("uploads/complete")]
    [Authorize]
    public async Task<IResult> CompleteUpload([FromBody] CompleteUploadRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        
        var result = await filesService.CompleteUpload(data);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        return Results.Ok(result.Data);
    }
}