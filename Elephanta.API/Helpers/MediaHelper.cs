using System;
using System.Collections.Generic;
using System.IO;
using Elephanta.Domain.Enums;

namespace Elephanta.API.Helpers;

public static class MediaHelper
{
    // Define allowed media types per module. Keys should be lower-case.
    private static readonly Dictionary<string, MediaType[]> AllowedMediaTypes = new()
    {
        ["product"] = new[] { MediaType.Image, MediaType.Video },
        ["category"] = new[] { MediaType.Image },
        ["offer"] = new[] { MediaType.Image, MediaType.Video },
        ["product-review"] = new[] { MediaType.Image },
        ["user"] = new[] { MediaType.Image }
    };

    public static MediaType GetMediaTypeFromContentType(string? contentType)
    {
        if (!string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return MediaType.Image;

        if (!string.IsNullOrWhiteSpace(contentType) && contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            return MediaType.Video;

        throw new InvalidOperationException("Unsupported media type.");
    }

    public static bool ValidateAllowed(string module, string? contentType, string? fileName, out MediaType detected, out MediaType[] allowed)
    {
        var key = (module ?? string.Empty).ToLowerInvariant();
        if (!AllowedMediaTypes.TryGetValue(key, out allowed))
        {
            allowed = new[] { MediaType.Image };
        }

        detected = GetMediaTypeFromContentType(contentType);
        if (Array.IndexOf(allowed, detected) < 0)
            return false;

        // Validate extension based on detected media type
        var ext = (fileName != null) ? Path.GetExtension(fileName).ToLowerInvariant() : string.Empty;
        if (detected == MediaType.Image)
        {
            var allowedImageExt = new[] { ".png", ".jpg", ".jpeg" };
            return Array.IndexOf(allowedImageExt, ext) >= 0;
        }

        if (detected == MediaType.Video)
        {
            var allowedVideoExt = new[] { ".mp4" };
            return Array.IndexOf(allowedVideoExt, ext) >= 0;
        }

        return false;
    }
}
