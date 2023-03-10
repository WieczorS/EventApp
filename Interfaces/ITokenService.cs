namespace interfaces;


public interface ITokenService
{
    string GenerateNewToken(int userId, IDictionary<string, object> claims, DateTime expiryDate);
    
}
