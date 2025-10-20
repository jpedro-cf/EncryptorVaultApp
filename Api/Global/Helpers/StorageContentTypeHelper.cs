using MyMVCProject.Api.Entities;

namespace MyMVCProject.Api.Global.Helpers;

public static class StorageContentTypeHelper
{
    private static readonly Dictionary<string, StorageContentType> Mapping = new(StringComparer.OrdinalIgnoreCase)
    {
        ["text"] = StorageContentType.Text,
        ["image"] = StorageContentType.Image,
        ["video"] = StorageContentType.Video,
        ["audio"] = StorageContentType.Audio,
        ["application"] = StorageContentType.Application
    };

    public static StorageContentType ToStorageContentType(this string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type cannot be null or empty.", nameof(contentType));

        if (!contentType.IsValidContentType())
        {
            throw new ArgumentException("Invalid content type.", nameof(contentType));
        }
        
        // "image/png" → "image"
        var mainType = contentType.Split('/')[0];

        return Mapping.GetValueOrDefault(mainType, StorageContentType.Application);
    }
}