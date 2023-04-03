using EventApp1.Models;
using interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventApp1.Controllers;
[ApiController]
[Route("[controller]")]
public class PasswordController:IPasswordResetService
{
    private readonly IPasswordResetService _passwordResetRepo;
    private readonly IUserService _userService;

    public PasswordController(IPasswordResetService passwordResetRepo, IUserService UserRepository)
    {
        _userService = UserRepository;
        _passwordResetRepo = passwordResetRepo;
    }


    [HttpPost]
  


    public Task<User> ResetPasswordMakeHash(ResetPasswordRequest request)
    {
        
        return _passwordResetRepo.ResetPasswordMakeHash(request);
        
    }
    [HttpPatch("{token123}")]
    public async Task<SetNewPasswordDto> SetNewPassword(NewPasswordUserRequest request, [FromRoute]string token123)
    {
        //var user = await _userService.GetUserByTokenAsync(token123);
        return await _passwordResetRepo.SetNewPassword(request, token123);

        //GetUserByTokenAsync UserRepository
    }
}

