using System.Net.Mime;
using System.Text;
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

    public static string FromBase64(this string value)
    {
        var bytes = Convert.FromBase64String(value);

        return Encoding.UTF8.GetString(bytes);
    }
}