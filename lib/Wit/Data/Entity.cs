using System.Collections.Generic;

namespace Wit.Data
{
    public record Entity(
        string Id,
        string Name,
        string Role,
        int? Start,
        int? End,
        string Body,
        double? Confidence,
        Dictionary<string, DynamicEntity[]> Entities,
        string Value,
        Resolved Resolved,
        string Type,
        DatePart From,
        DatePart To,
        ValuePart[] Values
    );
}