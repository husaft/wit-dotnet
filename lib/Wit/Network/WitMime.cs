using Wit.Data;

namespace Wit.Network
{
    internal static class WitMime
    {
        public static string ToContentType(FileType type)
        {
            var kind = type.ToString().ToLowerInvariant();
            var mime = $"audio/{kind}";
            return mime;
        }
    }
}