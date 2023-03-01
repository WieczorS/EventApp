using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
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

    
    

    
    [HttpPost]
    public IActionResult GenerateToken([FromBody] User user)
    {
        return Ok();
    }
[HttpGet]
    public async Task<UserDto> AuthenticateAsync(string username, string password)
    {
        var user = await _userServiceRepo.GetUserAsync(username);
        if (user == null)
        {
            return null;
        }

        return new UserDto { Id = user.id, Role = user.userTypeId, Username = user.login };
    }


}
