using System;

namespace Wit.Errors
{
    public sealed class WitError : Exception
    {
        public WitError(string message) : base(message)
        {
        }
    }
}