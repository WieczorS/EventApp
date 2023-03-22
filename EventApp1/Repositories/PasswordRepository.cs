using EventApp1.Models;
using interfaces;
using Microsoft.AspNetCore.Mvc;


namespace EventApp1.Repositories;

public class PasswordRepository : IPasswordResetService

{
private readonly IUserService _userRepo;

public PasswordRepository(IUserService repo)
{
    _userRepo = repo;
}

    [HttpPost("/api/reset-password")]
    public async Task<User> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        
        // Sprawdź, czy podany email istnieje w bazie danych
        var user = await _userRepo.GetUserByEmailAsync(request.Email);//pole email
        if (user == null)
        {
            throw new Exception("user not found");
        }

        // Generuj unikatowy token i zapisz go w bazie danych
        var token = Guid.NewGuid().ToString();
        await _userRepo.CreatePasswordResetToken(user.Id, token);

        // Wyślij email z linkiem do resetowania hasła
       // var resetPasswordLink = $"{_configuration.GetValue<string>("AppUrl")}/reset-password/{token}";
       // var emailBody = $"Witaj {user.Name},\n\nKliknij w link poniżej, aby zresetować swoje hasło:\n{resetPasswordLink}";
        //await _emailService.SendEmailAsync(request.Email, "Resetowanie hasła", emailBody);

       // return Ok(new { message = "Reset password email sent successfully" });
       return user;
    }
}