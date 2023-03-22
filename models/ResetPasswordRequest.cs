namespace EventApp1.Models;

public class ResetPasswordRequest
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
}