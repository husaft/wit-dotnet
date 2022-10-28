namespace Wit.Data
{
    public record SubEntity(
        string Entity,
        string Id,
        string Name,
        string Role,
        int? Start,
        int? End,
        string Body,
        double? Confidence,
        SubEntity[] Entities,
        string Value,
        Resolved Resolved,
        string Type,
        DatePart From,
        DatePart To,
        ValuePart[] Values
    );
}