using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventApp1.Config;
using EventApp1.Models;
using interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EventApp1.Services;

//constructor - adding jwtconfig from appsettings
public class JwtTokenService : ITokenService
{

    private readonly JwtConfig _config;
    
    public JwtTokenService(IOptions<JwtConfig> config)
    {
        this._config = config.Value;
    }
    

    public string GenerateNewToken(int userId, IDictionary<string, object> claims, DateTime expiryDate)
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
            Expires = expiryDate,
            Issuer = myIssuer,
            Audience = myAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature),
            Claims = claims
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        
        
        return tokenHandler.WriteToken(token);
        
        
    }
}
