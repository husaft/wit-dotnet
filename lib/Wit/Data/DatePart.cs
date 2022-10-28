using System;

namespace Wit.Data
{
    public record DatePart(
        string Grain,
        DateTime Value
    );
}