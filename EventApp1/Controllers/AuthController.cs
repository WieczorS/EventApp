using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using EventApp1.Config;
using EventApp1.Models;
using interfaces;
using Microsoft.Extensions.Options;

namespace EventApp1.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private JwtConfig _config;
    private readonly IUserService _userServiceRepo;

    public AuthController(IOptions<JwtConfig> config,IUserService repo)
    {
        _config = config.Value;
        _userServiceRepo = repo;
    }

    
    

    
    // [HttpPost]
    // public IActionResult GenerateToken([FromBody] User user)
    // {
    //     return Ok();
    // }
[HttpGet]
    public async Task<UserDto> AuthenticateAsync(string username, string password)
    {
        var user = await _userServiceRepo.GetUserAsync(username);
        if (user == null)
        {
            return null;
        }

        return new UserDto { Id = user.Id, Role = user.userTypeId, Username = user.login };
    }

    [HttpPost]
    public async Task<object?> AddUser(User user)
    {
      return await _userServiceRepo.AddUserAsync(user);
       
    }


}
