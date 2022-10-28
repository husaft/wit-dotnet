using System;

namespace Wit.Data
{
    public record AppVersion(
        string Name,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        string Desc
    );
}