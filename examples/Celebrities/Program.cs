using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wit;
using Wit.Data;
using Wit.Input;
using Wit.Tools;

namespace Celebrities
{
    internal static class Program
    {
        private static async Task<string> LoadWikiData(ResolvedPart celebrity)
        {
            string id;
            try
            {
                id = celebrity.External["wikidata"];
            }
            catch (Exception)
            {
                return $"I recognize {celebrity.Name}!";
            }
            var rsp = await _client.RequestExt("https://www.wikidata.org/w/api.php",
                new Dictionary<string, string>
                {
                    { "ids", id },
                    { "action", "wbgetentities" },
                    { "props", "descriptions" },
                    { "format", "json" },
                    { "languages", "en" }
                });
            var json = rsp.Single();
            var description = json["entities"]![id]!["descriptions"]!["en"]!["value"];
            return $"ooo yes I know {celebrity.Name} -- {description}";
        }

        private static async Task<string> HandleMessage(Meaning response)
        {
            var greetings = GetFirst(response.Traits, "wit$greetings");
            var celebrity = GetFirst(
                response.Entities, "wit$notable_person:notable_person"
            );
            if (celebrity != null)
                return await LoadWikiData(celebrity);
            if (greetings != null)
                return "Hi! You can say something like 'Tell me about Beyonce'";
            return "Um. I don't recognize that name. " +
                   "Which celebrity do you want to learn about?";
        }

        private static string GetFirst(IDictionary<string, Trait[]> traits, string trait)
        {
            if (!traits.ContainsKey(trait))
                return null;
            var tmp = traits[trait];
            var val = tmp[0].Value;
            return val;
        }

        private static ResolvedPart GetFirst(IDictionary<string, Entity[]> entities, string entity)
        {
            if (!entities.ContainsKey(entity))
                return null;
            var tmp = entities[entity];
            var val = tmp[0].Resolved.Values[0];
            return val;
        }

        private static WitClient _client;

        private static async Task Main(string[] args)
        {
            if (Config.Load().TryGetValue(typeof(Program).Namespace!, out var ct))
            {
                args = new[] { ct };
            }

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("usage: {0} <wit-access-token>", "celebrities");
                Console.WriteLine("Please import the corresponding .zip file upon creating " +
                                  "an app and grab your access token from the Settings page");
                Environment.Exit(1);
                return;
            }

            var accessToken = args[0];
            var logger = WitLog.Create(LogLevel.Trace);
            using var client = new WitClient(accessToken, logger);
            _client = client;
            await client.DoInteractive(HandleMessage);
        }
    }
}