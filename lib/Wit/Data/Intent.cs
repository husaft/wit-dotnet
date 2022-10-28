namespace Wit.Data
{
    public record Intent(
        string Id,
        string Name,
        double? Confidence,
        Entity[] Entities
    );
}