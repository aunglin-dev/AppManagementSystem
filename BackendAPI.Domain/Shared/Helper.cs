using System.Security.Cryptography;

namespace BackendAPI.Domain.Shared;

public class Helper
{

    private const int SaltSize = 16;        // 128 bit
    private const int KeySize = 32;         // 256 bit
    private const int Iterations = 100_000; // OWASP recommended


    public static void CreatePasswordHash(
       string password,
       out string passwordHash,
       out string passwordSalt)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(KeySize);

        passwordSalt = Convert.ToBase64String(salt);
        passwordHash = Convert.ToBase64String(hash);
    }

    public static bool VerifyPassword(
        string password,
        string storedHash,
        string storedSalt)
    {
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] storedHashBytes = Convert.FromBase64String(storedHash);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256);

        byte[] computedHash = pbkdf2.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(
            computedHash,
            storedHashBytes);
    }
}
