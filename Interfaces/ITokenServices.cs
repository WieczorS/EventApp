using Microsoft.AspNetCore.Mvc;

namespace interfaces;

public interface ITokenServices
{
    public IActionResult GenerateToken([FromBody])
}