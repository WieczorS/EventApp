namespace interfaces;

public interface IPasswordServices
{
    byte[] GenerateRandomSalt();
    string HashPassword(byte[] salt, string password);
}