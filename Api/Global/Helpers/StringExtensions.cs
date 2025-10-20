using System.Net.Mime;
using QRCoder;

namespace EncryptionApp.Api.Global.Helpers;

public static class StringExtensions
{
    public static string GenerateQrCodeBase64(this string value)
    {
        using var generator = new QRCodeGenerator();
        using var qrCodeData = generator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
            
        var qrCode = new PngByteQRCode(qrCodeData);

        var bytes = qrCode.GetGraphic(20);
            
        return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
    }
    
    public static string ToBase64(this byte[] value)
    {
        return Convert.ToBase64String(value);
    }

    public static bool IsValidContentType(this string value)
    {
        try
        {
            var contentType = new ContentType(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}