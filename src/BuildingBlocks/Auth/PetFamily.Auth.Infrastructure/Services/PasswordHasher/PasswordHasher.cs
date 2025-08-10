using PetFamily.Auth.Application.IServices;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.Security.Cryptography;

namespace PetFamily.Auth.Infrastructure.Services.PasswordHasher;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;        // 128 bit  
    private const int KeySize = 32;         // 256 bit  
    private const int Iterations = 100_000; // recommended minimum  

    public string Hash(string password)
    {
        var salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // count key PBKDF2-HMAC-SHA256  
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var key = pbkdf2.GetBytes(KeySize);

        // unite salt and key with ':'  
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(key)}";
    }

    public UnitResult Verify(string password, string hash)
    {
        // expect format "salt:hash"  
        var parts = hash.Split(':', 2);
        if (parts.Length != 2)
            return UnitResult.Fail(Error.InvalidFormat("hashed password"));

        var salt = Convert.FromBase64String(parts[0]);
        var storedKey = Convert.FromBase64String(parts[1]);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        var computedKey = pbkdf2.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(computedKey, storedKey) == true
            ? UnitResult.Ok()
            : UnitResult.Fail(Error.Custom("password.verify.error", "Error verify Password", ErrorType.Authentication));
    }
}
