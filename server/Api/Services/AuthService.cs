using EncryptionApp.Api.Dtos.Users;
using EncryptionApp.Api.Entities;
using EncryptionApp.Api.Global;
using EncryptionApp.Api.Global.Errors;
using EncryptionApp.Api.Infra.Security;
using EncryptionApp.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EncryptionApp.Api.Global.Helpers;

namespace EncryptionApp.Api.Services;

public class AuthService(
    AppDbContext ctx,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    JwtTokenHandler tokenHandler)
{
    public async Task<Result<LoginResponse>> Login(LoginRequest data)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
        if (user == null)
        {
             return Result<LoginResponse>.Failure(new UnauthorizedError("Incorrect e-mail or password."));
        }
        
        var result = await signInManager.PasswordSignInAsync(user, data.Password, false,false);
        if (!result.Succeeded && !result.RequiresTwoFactor)
        {
             return Result<LoginResponse>.Failure(new UnauthorizedError("Incorrect e-mail or password."));
        }

        if (result.RequiresTwoFactor)
        {
            return Result<LoginResponse>.Failure(new TwoFactorRequiredError("Two Factor authentication required."));
        }

        var jwt = tokenHandler.Encode(user, DateTime.Now.AddDays(5));

        return Result<LoginResponse>.Success(new LoginResponse(UserResponse.From(user), jwt, false));
    }

    public async Task<Result<LoginResponse>> LoginMfa(string code)
    {
        var codeDigits = new string(code.Where(char.IsDigit).ToArray());
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(codeDigits, false,false);
        
        if (!result.Succeeded)
        {
            return Result<LoginResponse>.Failure(new UnauthorizedError("Invalid code."));
        }

        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

        var jwt = tokenHandler.Encode(user!, DateTime.Now.AddDays(5));

        return Result<LoginResponse>.Success(new LoginResponse(UserResponse.From(user!), jwt, true));
    }

    public async Task<Result<MfaKeyResponse>> GetMfaKey(Guid userId)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result<MfaKeyResponse>.Failure(new NotFoundError("User not found."));
        }
        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (key == null)
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var totpUri = GetTotpUri(key!, user);

        return Result<MfaKeyResponse>.Success(new MfaKeyResponse(key!, totpUri.GenerateQrCodeBase64()));
    }

    public async Task<Result<bool>> SetupMfa(Guid userId, string token)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return Result<bool>.Failure(new NotFoundError("User not found."));
        }
        var tokenProvider = userManager.Options.Tokens.AuthenticatorTokenProvider;

        var tokenDigits = new string(token.Where(char.IsDigit).ToArray());
        var ok = await userManager.VerifyTwoFactorTokenAsync(user, tokenProvider, tokenDigits);
        if (!ok)
        {
            return Result<bool>.Failure(new UnauthorizedError("Token invalid."));
        }
        
        await userManager.SetTwoFactorEnabledAsync(user, true);
        return Result<bool>.Success(true);
    }

    private string GetTotpUri(string key, User user)
    {
        var provider = "EncryptionApp";

        return $"otpauth://totp/{user.Id}:{user.Email}?secret={key}&issuer={provider}";
    }
}