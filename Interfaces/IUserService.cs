using EventApp1.Models;


namespace interfaces;

public interface IUserService
{
    Task<User> AuthorizeUserAsync(string username, string password);
    Task<User> GetUserAsync(string username);
    Task AddUserAsync(UserAddDto user);
    Task<User> GetUserByEmailAsync(string email);

    Task CreatePasswordResetToken(int userId);
    Task<SetNewPasswordDto> GetUserByTokenAsync(string token);
  
}