using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Wit.Tools
{
    public static class WitJson
    {
        private static JsonSerializerSettings Create()
        {
            var resolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            var config = new JsonSerializerSettings
            {
                ContractResolver = resolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            return config;
        }

        private static readonly JsonSerializerSettings Config = Create();

        public static T Deserialize<T>(JToken token)
        {
            var json = token.ToString();
            var obj = Deserialize<T>(json);
            return obj;
        }

        public static T Deserialize<T>(string text)
        {
            var obj = JsonConvert.DeserializeObject<T>(text, Config);
            return obj;
        }

        public static string Serialize(object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Config);
            return json;
        }
    }
}