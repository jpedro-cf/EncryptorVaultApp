using System.Security.Cryptography;
using MyMVCProject.Api.Global;
using MyMVCProject.Api.Global.Errors;

namespace MyMVCProject.Api.Infra.Security;

public class EncryptionHandler
{
    public static Result<string> Encrypt(string base64Data, string key)
    {
        try
        {
            using var aes = Aes.Create();

            aes.Key = Convert.FromBase64String(key);
            aes.GenerateIV();

            byte[] encryptedData;

            using (var encryptor = aes.CreateEncryptor())
            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(base64Data);
                }

                encryptedData = msEncrypt.ToArray();
            }

            // Combine IV with the encrypted data
            var combined = aes.IV.Concat(encryptedData).ToArray();

            return Result<string>.Success(Convert.ToBase64String(combined));
        }
        catch (Exception e)
        {
            return Result<string>.Failure(new UnprocessableEntityError(e.Message));
        }
    }

    public static Result<string> Decrypt(string base64Data, string key)
    {
        var combined = Convert.FromBase64String(base64Data);

        // Extract IV and data
        var iv = combined[..16];
        var encryptedData = combined[16..];

        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(key);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var msDecrypt = new MemoryStream(encryptedData);
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        try
        {
            return Result<string>.Success(srDecrypt.ReadToEnd());
        }
        catch (CryptographicException ex)
        {
            return Result<string>.Failure(new UnprocessableEntityError("Decryption failed."));
        }
    }
    
    public static string GenerateRandomAes256Key()
    {
        byte[] key = new byte[32]; // 256 bits
        RandomNumberGenerator.Fill(key);
        return Convert.ToBase64String(key);
    }
}