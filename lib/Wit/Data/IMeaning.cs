using System.Collections.Generic;

namespace Wit.Data
{
    public interface IMeaning
    {
        Dictionary<string, Entity[]> Entities { get; }

        Dictionary<string, Trait[]> Traits { get; }
    }
}