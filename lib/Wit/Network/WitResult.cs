using System.IO;
using Newtonsoft.Json.Linq;

namespace Wit.Network
{
    public record WitResult
    (
        string Type,
        JObject Single,
        JObject[] Array,
        Stream Binary
    );
}