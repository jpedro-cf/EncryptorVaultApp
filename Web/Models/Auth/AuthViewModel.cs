namespace MyMVCProject.Web.Models.Auth;

public class AuthViewModel
{
    public LoginViewModel LoginModel { get; set; } = new LoginViewModel();
    public RegisterViewModel RegisterModel { get; set; } = new RegisterViewModel();
    public Tab ActiveTab { get; set; } = Tab.Login;

    public enum Tab
    {
        Login,
        Register
    }
}