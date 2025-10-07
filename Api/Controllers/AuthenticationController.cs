using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Api.Dtos.Users;
using MyMVCProject.Api.Entities;
using MyMVCProject.Api.Global.Exceptions;
using MyMVCProject.Api.Services;

namespace MyMVCProject.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(
    UsersService usersService, 
    AuthService authService, 
    SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterUserRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        
        var result = await usersService.Create(data);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        return Results.Created();

    }
    
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }

        var result = await authService.Login(data);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Expires = DateTime.Now.AddDays(5), 
            Path = "/", 
            Secure = true, 
            SameSite = SameSiteMode.Lax 
        };
        Response.Cookies.Append("accessToken", result.Data!.Token, cookieOptions);

        return Results.Ok(result.Data);

    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IResult> Logout()
    {
        Response.Cookies.Delete("accessToken");
        await signInManager.SignOutAsync();
        return Results.NoContent();
    }

    [HttpGet("mfa")]
    [Authorize]
    public async Task<IResult> GetMfaKey()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await authService.GetMfaKey(Guid.Parse(userId!));
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }
        
        return Results.Ok(result.Data!);
    }
    
    [HttpPost("mfa")]
    [Authorize]
    public async Task<IResult> SetupMfa([FromBody] SetupMfaRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await authService.SetupMfa(Guid.Parse(userId!), data.Token);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }
        
        return Results.NoContent();
    }
    
    [HttpPost("mfa/login")]
    public async Task<IResult> LoginMfa([FromBody] LoginMfaRequest data)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(data);
        }
        
        var result = await authService.LoginMfa(data.Code);
        if (!result.IsSuccess)
        {
            return result.Error!.ToHttpResult();
        }
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Expires = DateTime.Now.AddDays(5), 
            Path = "/", 
            Secure = true, 
            SameSite = SameSiteMode.Lax 
        };
        Response.Cookies.Append("accessToken", result.Data!.Token, cookieOptions);
        
        return Results.Ok(result.Data);
    }
}