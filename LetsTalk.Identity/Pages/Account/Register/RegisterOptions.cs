namespace LetsTalk.Identity.Pages.Register;

public class RegisterOptions
{
    public static readonly bool AllowLocalLogin = true;
    public static readonly bool AllowRememberLogin = true;
    public static readonly TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
    public static readonly string InvalidCredentialsErrorMessage = "Invalid username or password";
}