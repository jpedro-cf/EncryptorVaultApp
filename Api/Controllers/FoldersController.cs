using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Api.Dtos.Folders;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Api.Controllers;

[ApiController]
[Route("api/folders")]
public class FoldersController(FoldersService foldersService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IResult> Create([FromBody] CreateFolderRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await foldersService.Create(Guid.Parse(userId!), data);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }
        
        return Results.Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetFolder([FromRoute] Guid id, [FromQuery] GetFolderRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        var result = await foldersService.GetFolder(id, null, data);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        return Results.Ok(result.Data);
    }
}