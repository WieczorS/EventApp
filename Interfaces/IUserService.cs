using EventApp1.Models;

namespace interfaces;

public interface IUserService
{
    Task<User> AuthenticateAsync(string username, string password);
    Task<User> GetUserAsync(string username);
}