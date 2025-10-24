using EncryptionApp.Api.Global.Helpers;

namespace EncryptionApp.Api.Dtos.Items;

public record EncryptedData(string Data, string Iv)
{
    public static EncryptedData From(string value)
    {
        // [ iv | ciphertext ]
        var combined = Convert.FromBase64String(value);

        // Extract IV and data
        var iv = combined[..12];
        var encryptedData = combined[12..];

        return new EncryptedData(encryptedData.ToBase64(), iv.ToBase64());
    }
}