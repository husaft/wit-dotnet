namespace Wit.Data
{
    public record Trait(
        string trait,
        string Id,
        string Name,
        string Value,
        TraitValue[] Values,
        double? Confidence
    );
}