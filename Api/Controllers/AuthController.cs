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
public class AuthController(
    UsersService usersService, 
    AuthService authService, 
    SignInManager<User> signInManager) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(data);
        }
        
        await usersService.Create(data);

        return Created();

    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(data);
        }


        var result = await authService.Login(data);
        if (result.RequiresTwoFactor)
        {
            return Unauthorized(
                new TwoFactorRequiredException("Two Factor authentication required.")
                .ToProblemDetail());
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Expires = DateTime.Now.AddDays(5), 
            Path = "/", 
            Secure = true, 
            SameSite = SameSiteMode.Lax 
        };
        Response.Cookies.Append("accessToken", result.Token, cookieOptions);

        return Ok(result);

    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("accessToken");
        await signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet("mfa")]
    [Authorize]
    public async Task<IActionResult> GetMfaKey()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await authService.GetMfaKey(Guid.Parse(userId!));
        
        return Ok(result);
    }
    
    [HttpPost("mfa")]
    [Authorize]
    public async Task<IActionResult> SetupMfa([FromBody] SetupMfaRequest data)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await authService.SetupMfa(Guid.Parse(userId!),data.Token);
        
        return NoContent();
    }
    [HttpPost("mfa/login")]
    public async Task<IActionResult> LoginMfa([FromBody] LoginMfaRequest data)
    {
        var result = await authService.LoginMfa(data.Code);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, 
            Expires = DateTime.Now.AddDays(5), 
            Path = "/", 
            Secure = true, 
            SameSite = SameSiteMode.Lax 
        };
        Response.Cookies.Append("accessToken", result.Token, cookieOptions);
        
        return Ok(result);
    }
}