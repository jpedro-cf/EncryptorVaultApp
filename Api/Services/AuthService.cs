using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMVCProject.Api.Dtos.Users;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global.Exceptions;
using MyMVCProject.Api.Infra.Security;
using MyMVCProject.Api.Utils;
using MyMVCProject.Config;

namespace MyMVCProject.Api.Services;

public class AuthService(
    AppDbContext ctx,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    JwtTokenHandler tokenHandler)
{
    public async Task<LoginResponse> Login(LoginRequest data)
    {
        var user = await ctx.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
        if (user == null)
        {
            throw new UnauthorizedException("Incorrect e-mail or password.");
        }
        
        var result = await signInManager.PasswordSignInAsync(user, data.Password, false,false);
        if (!result.Succeeded && !result.RequiresTwoFactor)
        {
            throw new UnauthorizedException("Incorrect e-mail or password.");
        }

        if (result.RequiresTwoFactor)
        {
            return new LoginResponse(UserResponse.From(user), string.Empty, result.RequiresTwoFactor);
        }

        var jwt = tokenHandler.Encode(user, DateTime.Now.AddDays(5));

        return new LoginResponse(UserResponse.From(user), jwt, false);
    }

    public async Task<LoginResponse> LoginMfa(string code)
    {
        var codeDigits = new string(code.Where(char.IsDigit).ToArray());
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(codeDigits, false,false);
        
        if (!result.Succeeded)
        {
            throw new UnauthorizedException("Invalid code.");
        }

        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();

        var jwt = tokenHandler.Encode(user!, DateTime.Now.AddDays(5));

        return new LoginResponse(UserResponse.From(user!), jwt, true);
    }

    public async Task<MfaKeyResponse> GetMfaKey(Guid userId)
    {
        var user = await ctx.Users.FirstAsync(u => u.Id == userId);
        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (key == null)
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var totpUri = GetTotpUri(key!, user);

        return new MfaKeyResponse(key!, totpUri.GenerateQrCodeBase64());
    }

    public async Task SetupMfa(Guid userId, string token)
    {
        var user = await ctx.Users.FirstAsync(u => u.Id == userId);
        var tokenProvider = userManager.Options.Tokens.AuthenticatorTokenProvider;

        var tokenDigits = new string(token.Where(char.IsDigit).ToArray());
        var ok = await userManager.VerifyTwoFactorTokenAsync(user, tokenProvider, tokenDigits);
        if (!ok)
        {
            throw new UnauthorizedException("Token invalid.");
        }
        
        await userManager.SetTwoFactorEnabledAsync(user, true);
    }

    private string GetTotpUri(string key, User user)
    {
        var provider = "EncryptionApp";

        return $"otpauth://totp/{user.Id}:{user.Email}?secret={key}&issuer={provider}";
    }
}