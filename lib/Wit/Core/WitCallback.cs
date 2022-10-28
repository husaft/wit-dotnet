using System.Threading.Tasks;
using Wit.Data;

namespace Wit.Core
{
    public delegate Task<string> WitCallback(Meaning response);
}