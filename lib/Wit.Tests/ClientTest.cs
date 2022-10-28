using System;
using System.Linq;
using System.Threading.Tasks;
using Wit.Input;
using Wit.Tests.Util;
using Wit.Tools;
using Xunit;

namespace Wit.Tests
{
    public class ClientTest
    {
        [Fact]
        public async Task ShouldGetMeaning()
        {
            var token = Config.Load()["Basic"];
            using var client = new WitClient(token);
            const string msg = "set an alarm tomorrow at 7am";
            var mean = await client.GetMeaning(msg);
            Assert.Equal(msg, mean.Text);
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
        public async Task ShouldListUtterances()
        {
            var token = Config.Load()["Celebrities"];
            using var client = new WitClient(token);
            var utter = await client.ListUtterances();
            Assert.True(utter.Length >= 1);
            var basic = utter.First(a => a.Text == "hello");
            var trait = basic.Traits.Single();
            Assert.Equal("greetings", trait.Name);
            Assert.Equal("true", trait.Value);
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

        [Fact]
        public async Task ShouldGetExport()
        {
            var token = Config.Load()["Basic"];
            using var client = new WitClient(token);
            var url = await client.GetExportUrl();
            Assert.EndsWith("fbcdn.net", url.Host);
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