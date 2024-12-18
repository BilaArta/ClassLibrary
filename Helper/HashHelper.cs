using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ClassLibrary.Helper;
public class HashHelper
{
    private readonly IConfiguration _configuration;
    public HashHelper(IConfiguration configuration){
        _configuration = configuration;
    }
    
    public static (string hashedPassword, string salt) HashPasswordWithSalt(string password)
    {
        // Generate random salt
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);

        // Combine password and salt
        string saltedPassword = password + salt;

        // Hash the salted password
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return (hashedPassword, salt);
        }
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        // Combine entered password and stored salt
        string saltedPassword = enteredPassword + storedSalt;

        // Hash the salted password
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword == storedHash;
        }
    }
}
