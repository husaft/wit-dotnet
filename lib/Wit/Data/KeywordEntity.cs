namespace Wit.Data
{
    public record KeywordEntity(
        string Id,
        string Name,
        string[] Lookups,
        string[] Roles,
        DynamicEntity[] Keywords
    );
}