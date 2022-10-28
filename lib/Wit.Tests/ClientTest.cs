using Wit.Input;
using Wit.Tests.Util;
using Xunit;

namespace Wit.Tests
{
    public class ClientTest
    {
        [Fact]
        public void ShouldSendMessage()
        {
            var token = Config.Load()["Basic"];
            using var client = new WitClient(token);
            client.SendMessage("set an alarm tomorrow at 7am");
            Assert.NotNull(client);
        }

        [Fact]
        public void ShouldCustomLog()
        {
            var token = Config.Load()["Basic"];
            var logger = new TestLogger();
            using var client = new WitClient(token, logger);
            Assert.NotNull(client);
        }
    }
}