using System.Security.Cryptography;
using QRCoder;

namespace MyMVCProject.Api.Global;

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

    public static byte[] GetBytes(this string value)
    {
        return Convert.FromBase64String(value);
    }

    public static string ToBase64(this string value)
    {
        return Convert.ToBase64String(GetBytes(value));
    }
    
    public static string ToBase64(this byte[] value)
    {
        return Convert.ToBase64String(value);
    }

    public static string ToMd5Base64(this string value)
    {
        byte[] bytes = value.GetBytes();
        using (var md5 = MD5.Create())
        {
            byte[] md5Bytes = md5.ComputeHash(bytes);

            return Convert.ToBase64String(md5Bytes);
        }
    }
}