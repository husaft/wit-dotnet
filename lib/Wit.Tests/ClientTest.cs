using System.Linq;
using System.Threading.Tasks;
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

        [Theory]
        [InlineData("Tanıştığıma memnun oldum.", "tr_TR:1")]
        [InlineData("Je suis désolé.", "fr_XX:1")]
        public async Task ShouldDetectLang(string text, string expected)
        {
            var token = Config.Load()["Basic"];
            using var client = new WitClient(token);
            var one = (await client.DetectLanguage(text)).Single();
            var actual = $"{one.Locale}:{one.Confidence}";
            Assert.Equal(expected, actual);
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