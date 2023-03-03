using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using EventApp1.Config;
using EventApp1.Models;
using interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EventApp1.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private JwtConfig _config;
    private readonly IUserService _userServiceRepo;
    private readonly IPasswordServices _passwordServices;

    public AuthController(IOptions<JwtConfig> config,IUserService repo,IPasswordServices repo1)
    {
        _config = config.Value;
        _userServiceRepo = repo;
        _passwordServices = repo1;

    }

    
    

    
    [HttpPost]
    public IActionResult GenerateToken([FromBody] UserAddDto user)
    {
        if (user.password != "adam" && user.password != "adam")
        {
            return Unauthorized();
        }
        else
        {
            var mySecret = _config.Key;
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));
            var myIssuer = _config.Issuer;
            var myAudience = _config.Audience;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, 1.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = myIssuer,
                Audience = myAudience,
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(tokenHandler.WriteToken(token));
        }
        
    }
[HttpGet]
    public async Task<UserDto> AuthenticateAsync(string username, string password)
    {
        var user = await _userServiceRepo.GetUserAsync(username);
        var userPwd = _passwordServices.HashPassword(Convert.FromBase64String(user.passwordSalt),password);
        if(user.password == userPwd)

        {
            
            return new UserDto { Id = user.Id, Role = user.UserTypeId, Username = user.login };
        }
        else
        {
            
        
             return null;
        }


       
    }

    [HttpPut]
    public async Task AddUser(UserAddDto user)
    {
      await _userServiceRepo.AddUserAsync(user);
       
    }


}
