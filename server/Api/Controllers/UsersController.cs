using System.Security.Claims;
using EncryptionApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EncryptionApp.Api.Dtos.Users;

namespace EncryptionApp.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UsersService usersService, StorageUsageService storageUsageService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IResult> GetAccountData()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var user = await usersService.GetUserById(Guid.Parse(userId));
        var summary = await storageUsageService
            .GetStorageSummary(Guid.Parse(userId));

        return !user.IsSuccess ? user.Error!.ToHttpResult() : Results.Ok(new CurrentUserResponse(user.Data!, summary));
    }
    
    [HttpPatch("me/vault-key")]
    [Authorize]
    public async Task<IResult> UpdateVault([FromBody] UpdateVaultKeyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await usersService.UpdateVaultKey(Guid.Parse(userId), request);

        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.NoContent();
    }
    
    [HttpPut("me")]
    [Authorize]
    public async Task<IResult> Update([FromBody] UpdateAccountRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await usersService.UpdateAccount(Guid.Parse(userId), request);

        return !result.IsSuccess ? result.Error!.ToHttpResult() : Results.Ok(result.Data);
    }
}