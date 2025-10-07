namespace MyMVCProject.Web.Views.Utils;

public static class Utils
{
    public static string FormatFileSize(long bytes)
    {
        if (bytes <= 0)
            return "0 Bytes";

        var sizes = new[] { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        int i = (int)Math.Floor(Math.Log(bytes) / Math.Log(1024));
        double value = bytes / Math.Pow(1024, i);

        return $"{value:0.##} {sizes[i]}";
    }
}