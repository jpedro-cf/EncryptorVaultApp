using EncryptionApp.Api.Dtos.Users;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EncryptionApp.Api.Services;

public class UsersService(AppDbContext ctx, UserManager<User> userManager)
{
    public async Task<Result<bool>> Create(RegisterUserRequest data)
    {
        await using var transaction = await ctx.Database.BeginTransactionAsync();
        try
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

            foreach (ContentType contentType in Enum.GetValues(typeof(ContentType)))
            {
                var storageUsage = new StorageUsage
                {
                    UserId = identityUser.Id,
                    ContentType = contentType,
                    TotalSize = 0
                };

                ctx.StorageUsage.Add(storageUsage);
            }

            await ctx.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return Result<bool>.Failure(
                new InternalServerError("An error occured while creation your account."));
        }
    }

    public async Task<Result<UserResponse>> UpdateAccount(Guid userId, UpdateAccountRequest data)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result<UserResponse>.Failure(
                new NotFoundError("User not found."));
        }

        if (!await userManager.CheckPasswordAsync(user, data.CurrentPassword))
        {
            return Result<UserResponse>.Failure(
                new ForbiddenError("Incorrect password."));
        }
        
        if (!data.Email.IsNullOrEmpty())
        {
            var emailInUse = await userManager.FindByEmailAsync(data.Email!);
            if (emailInUse != null && emailInUse.Id != user.Id)
            {
                return Result<UserResponse>.Failure(
                    new EmailInUseError("E-mail already in use."));
            }

            await userManager.SetEmailAsync(user, data.Email!);
            await userManager.SetUserNameAsync(user, data.Email!);
        }

        if (!data.NewPassword.IsNullOrEmpty())
        {
            var hash = userManager.PasswordHasher.HashPassword(user, data.NewPassword!);
            user.PasswordHash = hash;
        }

        await ctx.SaveChangesAsync();
        
        return Result<UserResponse>.Success(UserResponse.From(user));
    }

    public async Task<Result<bool>> UpdateVaultKey(Guid userId, UpdateVaultKeyRequest data)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result<bool>.Failure(new NotFoundError("User not found."));
        }

        user.VaultKey = data.VaultKey;

        await ctx.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<UserResponse>> GetUserById(Guid userId)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result<UserResponse>.Failure(
                new NotFoundError("User not found."));
        }
        
        return Result<UserResponse>.Success(UserResponse.From(user));
    }
}