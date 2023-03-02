using System.Security.Cryptography;
using interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace EventApp1.Services;

public class PasswordServices:IPasswordServices
{
    private IPasswordServices _passwordServicesImplementation;

    public byte[] GenerateRandomSalt()
    {
        byte[] salt = new byte[8];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }
    

    public string HashPassword(byte[] salt, string password)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            iterationCount: 100000,
            prf: KeyDerivationPrf.HMACSHA256,
            numBytesRequested:256/8
            ));
        return hashed;
    }
}