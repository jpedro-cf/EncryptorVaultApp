using System.Security.Claims;
using EncryptionApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EncryptionApp.Api.Dtos.Users;

namespace EncryptionApp.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UsersService usersService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IResult> GetAccountData()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await usersService.GetUserById(Guid.Parse(userId));

        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.Ok(result.Data);
    }
    
    [HttpPatch("me/vault-key")]
    [Authorize]
    public async Task<IResult> UpdateVault([FromBody] string newKey)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await usersService.UpdateVaultKey(Guid.Parse(userId), newKey);

        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.NoContent();
    }
}