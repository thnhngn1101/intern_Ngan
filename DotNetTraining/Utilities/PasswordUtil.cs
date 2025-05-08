using System.Security.Cryptography;
using System.Text;
using Common.Application.Settings;


namespace Utilities
{
    public static class PasswordUtil
    {
        //Use PBKDF2 algorithm for hashing
        public static string HashPBKDF2(string password, PasswordSetting passwordSetting, out byte[] salt, byte[]? existSalt = null)
        {
            salt = existSalt ?? RandomNumberGenerator.GetBytes(passwordSetting.KeySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                passwordSetting.Iterations,
                passwordSetting.HashAlgorithmName,
                passwordSetting.KeySize);
            return Convert.ToHexString(hash);
        }

        public static bool VerifyPassword(string password, string hash, byte[] salt, PasswordSetting passwordSetting)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, passwordSetting.Iterations, passwordSetting.HashAlgorithmName, passwordSetting.KeySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
    public class HashingWithKeyService
    {
        private readonly string _secretKey;

        public HashingWithKeyService(IConfiguration configuration)
        {
            _secretKey = configuration["HashingOptions:SecretKey"]!;
        }

        public string HashPassword(string plainPassword)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            string newHash = HashPassword(plainPassword);
            return hashedPassword == newHash;
        }
    }
}
