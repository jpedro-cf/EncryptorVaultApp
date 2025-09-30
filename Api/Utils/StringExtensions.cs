using QRCoder;

namespace MyMVCProject.Api.Utils;

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
}