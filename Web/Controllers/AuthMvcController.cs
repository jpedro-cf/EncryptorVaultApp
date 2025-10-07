using Microsoft.AspNetCore.Mvc;
using MyMVCProject.Api.Global.Errors;
using MyMVCProject.Api.Services;
using MyMVCProject.Web.Models.Auth;

namespace MyMVCProject.Web.Controllers;

[Route("Auth")]
public class AuthMvcController(AuthService authService, UsersService usersService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new AuthViewModel());
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var indexModel = new AuthViewModel
        {
            LoginModel = model,
        };
        
        if (!ModelState.IsValid)
        {
            var message = ModelState.Values
                .SelectMany(v => v.Errors)
                .First()
                .ErrorMessage;
            
            indexModel.LoginModel.ErrorMessage = message;
            return View("Index", indexModel);
        }
        var result = await authService.Login(model);

        var twoFactorRequired = result.Error != null
                                && result.Error.GetType() == typeof(TwoFactorRequiredError);
        
        if (!result.IsSuccess && !twoFactorRequired)
        {
            model.ErrorMessage = result.Error!.Message;
            return View("Index", indexModel);
        }

        if (!result.IsSuccess && twoFactorRequired)
        {
            return Redirect("Auth/Mfa");
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
        
        return Redirect("/");
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var indexModel = new AuthViewModel
        {
            ActiveTab = AuthViewModel.Tab.Register,
            RegisterModel = model
        };

        if (!ModelState.IsValid)
        {
            var message = ModelState.Values
                .SelectMany(v => v.Errors)
                .First()
                .ErrorMessage;
            
            indexModel.RegisterModel.ErrorMessage = message;
            return View("Index", indexModel);
        }
        var result = await usersService.Create(model);

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.Error!.Message;
            return View("Index", indexModel);
        }

        return View("Index", new AuthViewModel());
    }
}