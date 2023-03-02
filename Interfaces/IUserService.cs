using EventApp1.Models;

namespace interfaces;

public interface IUserService
{
    Task<User> AuthorizeUserAsync(string username, string password);
    Task<User> GetUserAsync(string username);
    Task<object?> AddUserAsync(User user);
}