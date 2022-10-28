using System;

namespace Wit.Data
{
    public record Context(
        DateTime? ReferenceTime,
        string Timezone,
        string Locale,
        LatLng Coords
    );
}