using System.IO;

namespace Wit.Data
{
    public record FileObj(
        FileType Type,
        Stream Stream,
        byte[] Bytes
    );
}