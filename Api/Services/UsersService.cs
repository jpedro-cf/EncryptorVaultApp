using Microsoft.AspNetCore.Identity;
using MyMVCProject.Api.Dtos.Users;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global;
using MyMVCProject.Api.Global.Errors;
using MyMVCProject.Config;

namespace MyMVCProject.Api.Services;

public class UsersService(AppDbContext ctx, UserManager<User> userManager)
{
    public async Task<Result<bool>> Create(RegisterUserRequest data)
    {
        var identityUser = new User
        {
            UserName = data.Email,
            Email = data.Email,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(identityUser, data.Password);
        if (!result.Succeeded)
        {
            return Result<bool>.Failure(new UnauthorizedError(result.Errors.First().Description));
        }
        return Result<bool>.Success(true);
    }
}