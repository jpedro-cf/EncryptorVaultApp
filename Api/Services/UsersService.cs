using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Users;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global.Exceptions;
using MyMVCProject.Api.Infra.Security;
using MyMVCProject.Config;

namespace MyMVCProject.Api.Services;

public class UsersService(AppDbContext ctx, UserManager<User> userManager)
{
    public async Task Create(RegisterUserRequest data)
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
            throw new UnauthorizedException(result.Errors.First().Description);
        }
    }

    public async Task<User> FindById(Guid id)
    {
        return await ctx.Users.FirstAsync(u => u.Id == id);
    }

    public bool PasswordMatch(Guid userId, string password)
    {
        return true;
    }
}