using EncryptionApp.Api.Entities;

namespace EncryptionApp.Api.Global.Helpers;

public static class ContentTypeHelper
{
    private static readonly Dictionary<string, ContentType> Mapping = new(StringComparer.OrdinalIgnoreCase)
    {
        ["text"] = ContentType.Text,
        ["image"] = ContentType.Image,
        ["video"] = ContentType.Video,
        ["audio"] = ContentType.Audio,
        ["application"] = ContentType.Application
    };

    public static ContentType ToContentTypeEnum(this string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be null or empty.", nameof(contentType));

        if (!contentType.IsValidContentType())
        {
            throw new ArgumentException("Invalid content type.", nameof(contentType));
        }
        
        // "image/png" → "image"
        var mainType = contentType.Split('/')[0];

        return Mapping.GetValueOrDefault(mainType, ContentType.Application);
    }
    
    public static bool IsValidContentType(this string value)
    {
        try
        {
            var contentType = new System.Net.Mime.ContentType(value);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}