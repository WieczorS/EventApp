using Microsoft.AspNetCore.Mvc;
using EventApp1.Models;
using interfaces;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EventApp1.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthController : Controller
{
    
    private readonly IUserService _userServiceRepo;
    private readonly IPasswordServices _passwordServices;
    private readonly ITokenService _tokenService;
    private readonly ILogger _logger;

     public AuthController(ILogger<AuthController> logger,IUserService repo,IPasswordServices repo1, ITokenService repo2)
     {
         _logger = logger;
         _userServiceRepo = repo;
        _passwordServices = repo1;
        _tokenService = repo2;

    }



    [HttpPost]
    public async Task<IActionResult> AuthenticateAsync([FromBody] AuthRequest request)
    {
        
        _logger.LogDebug("Debug test");
        _logger.LogInformation("Info test");

        var user = await _userServiceRepo.GetUserAsync(request.Login);
        if (string.IsNullOrEmpty(user?.passwordSalt))
        {
            _logger.LogDebug("nullreference detected");
            return Unauthorized();
        }
        var userPwd = _passwordServices.HashPassword(Convert.FromBase64String(user.passwordSalt), request.Password);
        if (user.password != userPwd)
        {
            var log = new LoggerConfiguration();
                
            _logger.LogInformation("Returned unauthorized{request}",request);
            return Unauthorized();
        }

        var claims = new Dictionary<string, object>();
        claims.Add("name", user.name);
        claims.Add("surname",user.surname);
        _logger.LogInformation("returned token, authorized{request}",request);
        return Ok(_tokenService.GenerateNewToken(user.Id,claims,DateTime.Now.AddDays(7)));

    }

        [HttpPut]
        public async Task AddUser(UserAddDto user)
        {
            await _userServiceRepo.AddUserAsync(user);
        }
    
}
