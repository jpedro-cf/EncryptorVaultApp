using System.Security.Claims;
using EncryptionApp.Api.Dtos.Share;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EncryptionApp.Api.Controllers;

[ApiController]
[Route("api/share")]
public class ShareController(ShareService shareService): ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IResult> Create([FromBody] CreateSharedItemRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(request);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await shareService.CreateShare(Guid.Parse(userId), request);

        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.Ok(result.Data);
    }
    
    [HttpGet("{id}")]
    public async Task<IResult> Create([FromRoute] string id)
    {
        var result = await shareService.GetSharedContent(id);
        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.Ok(result.Data);
    }
    
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IResult> Delete([FromRoute] Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await shareService.DeleteShare(Guid.Parse(userId), id);

        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.NoContent();
    }
}