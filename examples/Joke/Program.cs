using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wit;
using Wit.Data;
using Wit.Input;
using Wit.Tools;

namespace Joke
{
    internal static class Program
    {
        private static readonly Dictionary<string, string[]> AllJokes = new()
        {
            {
                "chuck", new[]
                {
                    "Chuck Norris counted to infinity - twice.",
                    "Death once had a near-Chuck Norris experience."
                }
            },
            {
                "tech", new[]
                {
                    "Did you hear about the two antennas that got married? The ceremony was long and boring, but the reception was great!",
                    "Why do geeks mistake Halloween and Christmas? Because Oct 31 === Dec 25."
                }
            },
            {
                string.Empty, new[]
                {
                    "Why was the Math book sad? Because it had so many problems."
                }
            }
        };

        private static string GetFirst(IDictionary<string, Trait[]> traits,
            string trait)
        {
            if (!traits.ContainsKey(trait))
                return null;
            var raw = traits[trait];
            var tmp = raw[0];
            var val = tmp.Value;
            return val;
        }

        private static string GetFirst(IDictionary<string, Entity[]> entities,
            string entity)
        {
            if (!entities.ContainsKey(entity))
                return null;
            var raw = entities[entity];
            var tmp = raw[0];
            var val = tmp.Value;
            return val;
        }

        private static string HandleMessage(IMeaning response)
        {
            var entities = response.Entities;
            var traits = response.Traits;
            var getJoke = GetFirst(traits, "getJoke");
            var greetings = GetFirst(traits, "wit$greetings");
            var category = GetFirst(entities, "category:category");
            var sentiment = GetFirst(traits, "wit$sentiment");
            if (getJoke != null)
            {
                if (category != null)
                {
                    var jokes = AllJokes[category];
                    var index = Math.Floor(Random.Shared.NextDouble() * jokes.Length);
                    return jokes[(int)index];
                }
                else
                {
                    var jokes = AllJokes[string.Empty];
                    return jokes[0];
                }
            }
            if (greetings != null)
            {
                return "Hey this is joke bot :)";
            }
            if (sentiment != null && sentiment != "neutral")
            {
                var reply = sentiment == "positive" ? "Glad you liked it." : "Hmm.";
                return reply;
            }
            return "I can tell jokes! Say 'tell me a joke about tech'!";
        }

        private static async Task Main(string[] args)
        {
            if (Config.Load().TryGetValue(typeof(Program).Namespace!, out var ct))
            {
                args = new[] { ct };
            }

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("usage: {0} <wit-access-token>", "joke");
                Console.WriteLine("Please import the corresponding .zip file upon creating " +
                                  "an app and grab your access token from the Settings page");
                Environment.Exit(1);
                return;
            }

            var accessToken = args[0];
            var logger = WitLog.Create();
            using var client = new WitClient(accessToken, logger);
            await client.DoInteractive(HandleMessage);
        }
    }
}