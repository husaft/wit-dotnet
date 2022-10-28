namespace Wit.Data
{
    public record Utterance(
        string Text,
        string Intent,
        SubEntity[] Entities,
        Trait[] Traits
    );
}