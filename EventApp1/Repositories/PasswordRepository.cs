using System.Data;
using EventApp1.Models;
using interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;


namespace EventApp1.Repositories;

public class PasswordRepository : IPasswordResetService

{
private readonly IUserService _userRepo;
private readonly NpgsqlConnection _conn;
private readonly IPasswordServices _passwordServices;

public PasswordRepository(IUserService repo,NpgsqlConnection conn,IPasswordServices services)
{
    _passwordServices = services;
    _userRepo = repo;
    _conn = conn;

}

  
    public async Task<User> ResetPasswordMakeHash([FromBody] ResetPasswordRequest request)
    {
        
        // Sprawdź, czy podany email istnieje w bazie danych
        var user = await _userRepo.GetUserByEmailAsync(request.Email);//pole email
        if (user == null)
        {
            throw new Exception("user not found");
        }

        // Generuj unikatowy token i zapisz go w bazie danych
       
        await _userRepo.CreatePasswordResetToken(user.Id);

        // Wyślij email z linkiem do resetowania hasła
       // var resetPasswordLink = $"{_configuration.GetValue<string>("AppUrl")}/reset-password/{token}";
       // var emailBody = $"Witaj {user.Name},\n\nKliknij w link poniżej, aby zresetować swoje hasło:\n{resetPasswordLink}";
        //await _emailService.SendEmailAsync(request.Email, "Resetowanie hasła", emailBody);

       // return Ok(new { message = "Reset password email sent successfully" });
       return user;
    }

    

    public async Task<SetNewPasswordDto> SetNewPassword(NewPasswordUserRequest request,string token)
    {
        //sprawdzenie hasha if(hash == hash.db && mail == mail.db && token_exp_date)
        //zmien haslo na bazie na nowe oraz usuń wartości(opt)
        //jesli hash nie jest dobry to throw new exception
        
        //newPwDto obiekt typu setNewPasswordDto brany z bazy danych dla usera
        //request obiekt brany ze strony - nowy
        var newPwDto = await _userRepo.GetUserByTokenAsync(token);
        if (newPwDto.token.IsNullOrEmpty())
        {
            throw new Exception("brak hasha");
        }

        
        if (newPwDto.tokenExpDate < DateTime.Now)
        {
            throw new Exception("token is invalid");
        }

        if (_conn.State == ConnectionState.Closed)
        {
            _conn.Open();
        }
        await using (var cmd = new NpgsqlCommand(@"UPDATE users
                                                        SET token_exp_time = @date, password = @newpwd, token = @token
                                                        WHERE id = @id;", _conn))
        {
            var userPwd = _passwordServices.HashPassword(Convert.FromBase64String(newPwDto.salt), request.newPasswordInput);
            cmd.Parameters.AddWithValue("newpwd", userPwd);
            cmd.Parameters.AddWithValue("token", "pwdchanged");
            cmd.Parameters.AddWithValue("id", newPwDto.id);
            cmd.Parameters.AddWithValue("date",DateTime.Now);
             await cmd.ExecuteScalarAsync();
        }

        return newPwDto;
    }
    
}

public class NewPaasswordUserRequest
{
}