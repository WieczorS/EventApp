using EventApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace interfaces;

public interface IPasswordResetService
{
    Task<User> ResetPasswordMakeHash([FromBody] ResetPasswordRequest request);
    Task<SetNewPasswordDto> SetNewPassword([FromBody] NewPasswordUserRequest request,[FromRoute]string token);
}

