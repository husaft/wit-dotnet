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
        public async Task ShouldListApps()
        {
            var token = Config.Load()["Basic"];
            using var client = new WitClient(token);
            var apps = await client.ListApps();
            Assert.True(apps.Length >= 1);
            var basic = apps.First(a => a.Name == "basics_bot");
            Assert.Equal("en", basic.Lang);
            Assert.True(basic.Private);
        }

        [Fact]
        public async Task ShouldGetApp()
        {
            var token = Config.Load()["Basic"];
            using var client = new WitClient(token);
            var appId = (await client.ListApps()).First().Id;
            var basic = await client.GetAppInfo(appId);
            Assert.Equal("en", basic.Lang);
            Assert.True(basic.Private);
        }

        // throw new InvalidOperationException(WitJson.Serialize(apps));

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