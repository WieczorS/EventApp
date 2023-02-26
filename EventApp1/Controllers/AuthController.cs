using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using EventApp1.Config;
using Microsoft.Extensions.Options;

namespace EventApp1.Controllers;
[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private JwtConfig _config;

    public AuthController(IOptions<JwtConfig> config)
    {
        _config = config.Value;
    }
    

}
