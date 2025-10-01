using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyMVCProject.Api.Infra.Security;

public class KeyDerivationHandler
{
    public static (string salt, string key) CreateDerivedKey(string secret)
    {
        var salt = RandomNumberGenerator.GetBytes(16); // 128 bits

        var pbkdf2 = KeyDerivation.Pbkdf2(
            password: secret,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 32); // 256 bits

        return (Convert.ToBase64String(salt),Convert.ToBase64String(pbkdf2));
    }
}