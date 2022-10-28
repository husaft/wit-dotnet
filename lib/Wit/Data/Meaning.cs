using System.Collections.Generic;

namespace Wit.Data
{
    public record Meaning(
        string Text,
        Intent[] Intents,
        Dictionary<string, Entity[]> Entities,
        Dictionary<string, Trait[]> Traits
    );
}