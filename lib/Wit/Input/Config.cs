using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Wit.Tools;

namespace Wit.Input
{
    public static class Config
    {
        public static IDictionary<string, string> Load(
            string fileName = "secrets.json")
        {
            if (!File.Exists(fileName))
                return new Dictionary<string, string>();

            var text = File.ReadAllText(fileName, Encoding.UTF8);
            var json = WitJson.Deserialize<JObject>(text);
            var vault = (JObject)json["KeyVault"];
            var dict = vault!.ToObject<Dictionary<string, string>>();
            return dict;
        }
    }
}