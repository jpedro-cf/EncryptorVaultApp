using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Api.Dtos.Users;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(UsersService usersService) : ControllerBase
{
    
    [HttpPatch("vault")]
    [Authorize]
    public async Task<IResult> UpdateVault([FromBody] string salt)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await usersService.UpdateVaultKey(Guid.Parse(userId), salt);

        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        return Results.NoContent();
    }
}