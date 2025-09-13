using System.Security.Cryptography;
using System.Text;

namespace EShopCase.Application.Helpers;

public class PasswordHash
{
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hashedBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
    public static bool VerifyPassword(string userInputPassword, string hashedPassword)
    {
        string userInputHashedPassword = HashPassword(userInputPassword);
        return userInputHashedPassword == hashedPassword;
    }
}