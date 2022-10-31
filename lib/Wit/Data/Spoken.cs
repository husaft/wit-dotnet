using System.Collections.Generic;

namespace Wit.Data
{
    public record Spoken(
        Dictionary<string, Entity[]> Entities,
        Intent[] Intents,
        bool? IsFinal,
        FoundSpeech Speech,
        string Text,
        Dictionary<string, Trait[]> Traits
    ) : IMeaning;
}