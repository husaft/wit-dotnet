using System.Collections.Generic;

namespace Wit.Data
{
    public record ResolvedPart(
        string Name,
        string Domain,
        Dictionary<string, string> External,
        Dictionary<string, string> Attributes
    );
}