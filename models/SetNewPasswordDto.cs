namespace EventApp1.Models;

public class SetNewPasswordDto
{
    public string token { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public string newPassword { get; set; }
    public string salt { get; set; }
    public DateTime tokenExpDate { get; set; }
    public string email { get; set; }
}