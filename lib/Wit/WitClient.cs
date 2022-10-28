using System;
using Microsoft.Extensions.Logging;

namespace Wit
{
    public sealed class WitClient : IDisposable
    {
        private readonly string _accessToken;
        private readonly ILogger _log;

        public WitClient(string accessToken, ILogger log = null)
        {
            _accessToken = accessToken;
            _log = log;
        }

        public void Dispose()
        {
        }

        public void SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}