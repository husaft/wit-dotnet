using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wit.Core;
using Wit.Data;
using Wit.Errors;
using Wit.Input;
using Wit.Network;
using Wit.Tools;

namespace Wit
{
    /// <summary>
    /// The central access point for this library.
    /// </summary>
    public sealed class WitClient : IDisposable
    {
        private readonly string _accessToken;
        private readonly ILogger _log;
        private readonly HttpClient _http;

        /// <summary>
        /// Create a new client instance.
        /// </summary>
        /// <param name="accessToken">the access token</param>
        /// <param name="log">optional custom logger</param>
        public WitClient(string accessToken, ILogger log = null)
        {
            _accessToken = accessToken;
            _log = log;
            _http = new HttpClient();
        }

        /// <summary>
        /// Clean up all the resources.
        /// </summary>
        public void Dispose()
        {
            _http.Dispose();
        }

        /// <summary>
        /// Requests some external URL.
        /// </summary>
        /// <param name="path">the web address</param>
        /// <param name="query">the query parameters</param>
        public Task<WitResult> RequestExt(string path, IDictionary<string, string> query = null)
            => Request(path, query: query, isApiCall: false);

        public string WitApiHost { get; set; } = WitConstants.DefaultWitUrl;
        public string WitApiVersion { get; set; } = WitConstants.DefaultApiVersion;
        public string InteractivePrompt { get; set; } = "> ";

        private async Task<WitResult> Request(string path, HttpMethod method = null,
            IDictionary<string, string> query = null, IDictionary<string, string> headers = null,
            bool isApiCall = true, FileObj binary = null, object payload = null)
        {
            method ??= HttpMethod.Get;
            var baseUrl = new Uri((isApiCall ? WitApiHost : string.Empty) + path);
            _log?.LogDebug("{0} {1} {2}", method, baseUrl, query);

            var fullUrl = baseUrl.AddQuery(query);
            using var request = new HttpRequestMessage(method, fullUrl).AddHeaders(headers);

            if (isApiCall)
            {
                var auth = new AuthenticationHeaderValue("Bearer", _accessToken);
                var acc = new MediaTypeWithQualityHeaderValue($"application/vnd.wit.{WitApiVersion}+json");
                request.Headers.Authorization = auth;
                request.Headers.Accept.Add(acc);
            }

            if (payload != null)
            {
                const string jsonMime = "application/json";
                var jsonTxt = WitJson.Serialize(payload);
                var content = new StringContent(jsonTxt, Encoding.UTF8, jsonMime);
                content.Headers.ContentType = new MediaTypeHeaderValue(jsonMime);
                request.Content = content;
            }

            if (binary != null)
            {
                var binaryMime = WitMime.ToContentType(binary.Type);
                HttpContent content = binary.Stream != null
                    ? new StreamContent(binary.Stream)
                    : new ByteArrayContent(binary.Bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue(binaryMime);
                request.Content = content;
            }

            using var response = await _http.SendAsync(request);
            var result = await WitHttp.ParseResult(response.Content);
            var json = result.Array?.FirstOrDefault() ?? result.Single;

            var statusCode = (int)response.StatusCode;
            if (statusCode > 200)
            {
                var error = json?["error"]?.ToString();
                var reason = $"{response.ReasonPhrase} {response.Headers.WwwAuthenticate} {error}";
                throw new WitError($"[{statusCode}] {reason}".Trim());
            }

            var fmt = json?.ToString(Formatting.None);
            _log?.LogDebug("{0} {1} {2}", method, fullUrl, fmt);
            return result;
        }

        /// <summary>
        /// Returns the meaning of the text message.
        /// </summary>
        /// <param name="msg">the text message</param>
        /// <param name="context">optional context</param>
        public async Task<Meaning> GetMeaning(string msg,
            IDictionary<string, string> context = null)
        {
            var query = new Dictionary<string, string>();
            if (msg != null)
                query["q"] = msg;
            if (context != null)
                query["context"] = WitJson.Serialize(context);
            var rsp = (await Request("/message", query: query)).Single;
            var item = rsp.ToObject<Meaning>();
            return item;
        }

        /// <summary>
        /// Sends an audio file to the speech API.
        /// </summary>
        /// <param name="file">an audio file</param>
        public async Task<Spoken[]> Speech(FileObj file)
        {
            var query = new Dictionary<string, string>();
            var rsp = (await Request("/speech", HttpMethod.Post, query, binary: file)).Array;
            var list = rsp?.Select(WitJson.Deserialize<Spoken>);
            return list?.ToArray() ?? Array.Empty<Spoken>();
        }

        /// <summary>
        /// Sends an audio file to the dictation API.
        /// </summary>
        /// <param name="file">an audio file</param>
        public async Task<Speech[]> Dictate(FileObj file)
        {
            var query = new Dictionary<string, string>();
            var rsp = (await Request("/dictation", HttpMethod.Post, query, binary: file)).Array;
            var list = rsp?.Select(WitJson.Deserialize<Speech>);
            return list?.ToArray() ?? Array.Empty<Speech>();
        }

        /// <summary>
        /// Returns the list of the top detected locales for the text message.
        /// </summary>
        /// <param name="msg">the text message</param>
        public async Task<DetectedLocale[]> DetectLanguage(string msg)
        {
            var query = new Dictionary<string, string> { ["q"] = msg };
            var res = await Request("/language", query: query);
            var rsp = res.Single["detected_locales"];
            return rsp?.ToObject<DetectedLocale[]>();
        }

        /// <summary>
        /// Send your text to be synthesized.
        /// </summary>
        /// <param name="msg">the text message</param>
        /// <param name="voice">the voice</param>
        /// <param name="style">the style</param>
        /// <param name="speed">its speed</param>
        /// <param name="pitch">its pitch</param>
        /// <param name="gain">its gain</param>
        public async Task<Stream> Synthesize(string msg, string voice = "Rebecca",
            string style = "default", int speed = 100, int pitch = 100, int gain = 100)
        {
            var query = new Dictionary<string, string>
            {
                ["q"] = msg, ["voice"] = voice, ["style"] = style,
                ["speed"] = speed + string.Empty,
                ["pitch"] = pitch + string.Empty,
                ["gain"] = gain + string.Empty
            };
            var rsp = await Request("/synthesize", HttpMethod.Post, payload: query);
            return rsp.Binary;
        }

        /// <summary>
        /// Returns an array of all your apps.
        /// </summary>
        /// <param name="limit">number of apps to return</param>
        /// <param name="offset">number of utterances to skip</param>
        public async Task<AppInfo[]> ListApps(int? limit = 100, int? offset = null)
        {
            var query = new Dictionary<string, string>();
            if (limit is not null)
                query["limit"] = limit + string.Empty;
            if (offset is not null)
                query["offset"] = offset + string.Empty;
            var rsp = (await Request("/apps", query: query)).Array;
            var list = rsp.Select(WitJson.Deserialize<AppInfo>);
            return list.ToArray();
        }

        /// <summary>
        /// Returns an object representation of the specified app.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        public async Task<AppInfo> GetAppInfo(string appId)
        {
            var endpoint = $"/apps/{WitHttp.Encode(appId)}";
            var rsp = (await Request(endpoint)).Single;
            var item = rsp.ToObject<AppInfo>();
            return item;
        }

        /// <summary>
        /// Returns an object representation of the specified app version.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        /// <param name="tagId">name of tag</param>
        public async Task<AppVersion> GetAppVersionInfo(string appId, string tagId)
        {
            var endpoint = $"/apps/{WitHttp.Encode(appId)}/tags/{WitHttp.Encode(tagId)}";
            var rsp = (await Request(endpoint)).Single;
            var item = rsp.ToObject<AppVersion>();
            return item;
        }

        /// <summary>
        /// Create a new version of your app.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        /// <param name="tagName">name of tag</param>
        public async Task<AppInfo> CreateAppVersion(string appId, string tagName)
        {
            var data = new JObject { { "tag", tagName } };
            var endpoint = $"/apps/{appId}/tags/";
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<AppInfo>();
            return item;
        }

        /// <summary>
        /// Creates a new intent with the given attributes.
        /// </summary>
        /// <param name="intentName">name of intent to be created</param>
        public async Task<Intent> CreateIntent(string intentName)
        {
            var data = new JObject { { "name", intentName } };
            var endpoint = "/intents";
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<Intent>();
            return item;
        }

        /// <summary>
        /// Creates a new intent with the given attributes.
        /// </summary>
        /// <param name="entityName">name of entity to be created</param>
        /// <param name="roles">list of roles you want to create for the entity</param>
        /// <param name="lookups">list of lookup strategies</param>
        public async Task<Entity> CreateEntity(string entityName, string[] roles, string[] lookups = null)
        {
            var data = new JObject { { "name", entityName }, { "roles", new JArray(roles) } };
            var endpoint = "/entities";
            if (lookups != null)
                data["lookups"] = new JArray(lookups);
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<Entity>();
            return item;
        }

        /// <summary>
        /// Updates the attributes of an entity.
        /// </summary>
        /// <param name="currentEntityName">name of entity to be updated</param>
        /// <param name="newEntityName">new name of entity</param>
        /// <param name="roles">updated list of roles</param>
        /// <param name="lookups">updated list of lookup strategies</param>
        public async Task<Entity> UpdateEntity(string currentEntityName,
            string newEntityName, string[] roles, string[] lookups = null)
        {
            var data = new JObject { { "name", newEntityName }, { "roles", new JArray(roles) } };
            var endpoint = $"/entities/{WitHttp.Encode(currentEntityName)}";
            if (lookups != null)
                data["lookups"] = new JArray(lookups);
            var rsp = (await Request(endpoint, HttpMethod.Put, payload: data)).Single;
            var item = rsp.ToObject<Entity>();
            return item;
        }

        /// <summary>
        /// Add a possible value into the list of keywords for the keywords entity.
        /// </summary>
        /// <param name="entityName">name of entity to which keyword is to be added</param>
        /// <param name="data">the data to add</param>
        public async Task<ValuePart> AddKeywordValue(string entityName,
            IDictionary<string, string> data)
        {
            var endpoint = $"/entities/{WitHttp.Encode(entityName)}/keywords";
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<ValuePart>();
            return item;
        }

        /// <summary>
        /// Create a new synonym of the canonical value of the keywords entity.
        /// </summary>
        /// <param name="entityName">name of entity to which synonym is to be added</param>
        /// <param name="keywordName">name of keyword to which synonym is to be added</param>
        /// <param name="synonym">name of synonym to be created</param>
        public async Task<string> CreateSynonym(string entityName, string keywordName, string synonym)
        {
            var endpoint = $"/entities/{WitHttp.Encode(entityName)}/keywords/{WitHttp.Encode(keywordName)}/synonyms";
            var data = new JObject { { "synonym", synonym } };
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<string>();
            return item;
        }

        /// <summary>
        /// Creates a new trait with the given attributes.
        /// </summary>
        /// <param name="traitName">name of trait to be created</param>
        /// <param name="values">list of values for the trait</param>
        public async Task<Trait> CreateTrait(string traitName, string[] values)
        {
            var data = new JObject { { "name", traitName }, { "values", new JArray(values) } };
            var endpoint = "/traits";
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<Trait>();
            return item;
        }

        /// <summary>
        /// Creates a new trait value with the given attributes.
        /// </summary>
        /// <param name="traitName">name of trait to which new value is to be added</param>
        /// <param name="newValue">name of new trait value</param>
        public async Task<TraitValue> CreateTraitValue(string traitName, string newValue)
        {
            var data = new JObject { { "value", newValue } };
            var endpoint = $"/traits/{WitHttp.Encode(traitName)}/values";
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<TraitValue>();
            return item;
        }

        /// <summary>
        /// Train your utterances.
        /// </summary>
        /// <param name="data">array of utterances with required arguments</param>
        public async Task<Utterance> Train(string[] data)
        {
            var endpoint = "/utterances";
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<Utterance>();
            return item;
        }

        /// <summary>
        /// Creates a new app for an existing user.
        /// </summary>
        /// <param name="appName">name of new app</param>
        /// <param name="lang">language code in ISO 639-1 format</param>
        /// <param name="isPrivate">private if true</param>
        /// <param name="timeZone">default timezone of the app</param>
        public async Task<AppInfo> CreateApp(string appName, string lang, bool isPrivate, string timeZone = null)
        {
            var data = new JObject { { "name", appName }, { "lang", lang }, { "private", isPrivate } };
            var endpoint = "/apps";
            var query = new Dictionary<string, string>();
            if (timeZone is not null)
                query["timezone"] = timeZone;
            var rsp = (await Request(endpoint, HttpMethod.Post, payload: data)).Single;
            var item = rsp.ToObject<AppInfo>();
            return item;
        }

        /// <summary>
        /// Updates existing app with given attributes.
        /// </summary>
        /// <param name="appId">the id of the app</param>
        /// <param name="appName">new name</param>
        /// <param name="lang">language code in ISO 639-1 format</param>
        /// <param name="isPrivate">private if true</param>
        /// <param name="timeZone">default timezone of the app</param>
        public async Task<AppInfo> UpdateApp(string appId, string appName = null,
            string lang = null, bool? isPrivate = null, string timeZone = null)
        {
            var data = new JObject();
            var endpoint = $"/apps/{WitHttp.Encode(appId)}";
            if (appName != null)
                data["name"] = appName;
            if (lang != null)
                data["lang"] = lang;
            if (isPrivate != null)
                data["private"] = isPrivate;
            if (timeZone != null)
                data["timezone"] = timeZone;
            var rsp = (await Request(endpoint, HttpMethod.Put, payload: data)).Single;
            var item = rsp.ToObject<AppInfo>();
            return item;
        }

        /// <summary>
        /// Update the tag's name or description, or move the tag to point to another tag.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        /// <param name="tagName">name of existing tag</param>
        /// <param name="newName">name of new tag</param>
        /// <param name="desc">new description of tag</param>
        /// <param name="moveTo">new name of tag</param>
        public async Task<AppVersion> UpdateAppVersion(string appId, string tagName,
            string newName = null, string desc = null, string moveTo = null)
        {
            var data = new JObject();
            var endpoint = $"/apps/{WitHttp.Encode(appId)}/tags/{WitHttp.Encode(tagName)}";
            if (newName != null)
                data["tag"] = newName;
            if (desc != null)
                data["desc"] = desc;
            if (moveTo != null)
                data["move_to"] = moveTo;
            var rsp = (await Request(endpoint, HttpMethod.Put, payload: data)).Single;
            var item = rsp.ToObject<AppVersion>();
            return item;
        }

        /// <summary>
        /// Returns all available information about a trait.
        /// </summary>
        /// <param name="traitName">name of existing trait</param>
        public async Task<Trait> GetTraitInfo(string traitName)
        {
            var endpoint = $"/traits/{WitHttp.Encode(traitName)}";
            var rsp = (await Request(endpoint)).Single;
            var item = rsp.ToObject<Trait>();
            return item;
        }

        /// <summary>
        /// Delete an intent associated with your app.
        /// </summary>
        /// <param name="intentName">name of intent to be deleted</param>
        public async Task<DeletedResult> DeleteIntent(string intentName)
        {
            var endpoint = $"/intents/{WitHttp.Encode(intentName)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Returns an object representation of the specified app.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        public async Task<DeletedResult> DeleteApp(string appId)
        {
            var endpoint = $"/apps/{WitHttp.Encode(appId)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Delete a specific version of your app.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        /// <param name="tagName">name of tag</param>
        public async Task<DeletedResult> DeleteAppVersion(string appId, string tagName)
        {
            var endpoint = $"/apps/{WitHttp.Encode(appId)}/tags/{WitHttp.Encode(tagName)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Delete a trait associated with your app.
        /// </summary>
        /// <param name="traitName">name of trait to be deleted</param>
        public async Task<DeletedResult> DeleteTrait(string traitName)
        {
            var endpoint = $"/traits/{WitHttp.Encode(traitName)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Delete utterances from your app.
        /// </summary>
        /// <param name="utterances">list of utterances to be deleted</param>
        public async Task<DeletedResult> DeleteUtterances(string[] utterances)
        {
            var data = new List<JObject>();
            foreach (var utterance in utterances)
                data.Add(new JObject { { "text", utterance } });
            var rsp = (await Request("/utterances", HttpMethod.Delete, payload: data)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Deletes a value associated with the trait.
        /// </summary>
        /// <param name="traitName">name of trait whose particular value is to be deleted</param>
        /// <param name="valueName">name of value to be deleted</param>
        public async Task<DeletedResult> DeleteTraitValue(string traitName, string valueName)
        {
            var endpoint = $"/traits/{WitHttp.Encode(traitName)}/values/{WitHttp.Encode(valueName)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Delete a synonym of the keyword of the entity.
        /// </summary>
        /// <param name="entityName">name of entity whose particular keyword is to be deleted</param>
        /// <param name="keywordName">name of keyword to be deleted</param>
        /// <param name="synonymName">name of synonym to be deleted</param>
        public async Task<DeletedResult> DeleteSynonym(string entityName, string keywordName, string synonymName)
        {
            var endpoint = $"/entities/{WitHttp.Encode(entityName)}/keywords/{WitHttp.Encode(keywordName)}/synonyms/{WitHttp.Encode(synonymName)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Deletes a keyword associated with the entity.
        /// </summary>
        /// <param name="entityName">name of entity whose particular keyword is to be deleted</param>
        /// <param name="keywordName">name of keyword to be deleted</param>
        public async Task<DeletedResult> DeleteKeyword(string entityName, string keywordName)
        {
            var end = $"/entities/{WitHttp.Encode(entityName)}/keywords/{WitHttp.Encode(keywordName)}";
            var rsp = (await Request(end, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Deletes a role associated with the entity.
        /// </summary>
        /// <param name="entityName">name of entity whose particular role is to be deleted</param>
        /// <param name="roleName">name of role to be deleted</param>
        public async Task<DeletedResult> DeleteRole(string entityName, string roleName)
        {
            var end = $"/entities/{WitHttp.Encode(entityName)}:{WitHttp.Encode(roleName)}";
            var rsp = (await Request(end, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Delete an entity associated with your app.
        /// </summary>
        /// <param name="entityName">name of entity to be deleted</param>
        public async Task<DeletedResult> DeleteEntity(string entityName)
        {
            var endpoint = $"/entities/{WitHttp.Encode(entityName)}";
            var rsp = (await Request(endpoint, HttpMethod.Delete)).Single;
            var item = rsp.ToObject<DeletedResult>();
            return item;
        }

        /// <summary>
        /// Returns all available information about an entity.
        /// </summary>
        /// <param name="entityName">name of existing entity</param>
        public async Task<Entity> GetEntityInfo(string entityName)
        {
            var endpoint = $"/entities/{WitHttp.Encode(entityName)}";
            var rsp = (await Request(endpoint)).Single;
            var item = rsp.ToObject<Entity>();
            return item;
        }

        /// <summary>
        /// Returns all available information about an intent.
        /// </summary>
        /// <param name="intentName">name of existing intent</param>
        public async Task<Intent> GetIntentInfo(string intentName)
        {
            var endpoint = $"/intents/{WitHttp.Encode(intentName)}";
            var rsp = (await Request(endpoint)).Single;
            var item = rsp.ToObject<Intent>();
            return item;
        }

        /// <summary>
        /// Get a URL where you can download a ZIP file containing all of your app data.
        /// </summary>
        public async Task<Uri> GetExportUrl()
        {
            var rsp = (await Request("/export")).Single;
            var url = rsp["uri"]!.Value<string>();
            var uri = new Uri(url!);
            return uri;
        }

        /// <summary>
        /// Create a new app with all the app data from the exported app.
        /// </summary>
        /// <param name="name">name of the new app</param>
        /// <param name="isPrivate">private if true</param>
        /// <param name="file">archive file to upload</param>
        public async Task<AppInfo> ImportApp(string name, bool isPrivate, FileObj file)
        {
            var query = new Dictionary<string, string>
            {
                ["name"] = name,
                ["private"] = isPrivate + string.Empty
            };
            var rsp = (await Request("/import", HttpMethod.Post, query, binary: file)).Single;
            var item = rsp.ToObject<AppInfo>();
            return item;
        }

        /// <summary>
        /// Returns a JSON array of utterances.
        /// </summary>
        /// <param name="limit">number of utterances to return</param>
        /// <param name="offset">number of utterances to skip</param>
        /// <param name="intents">list of intents to filter the utterances</param>
        public async Task<Utterance[]> ListUtterances(int? limit = 100,
            int? offset = null, string[] intents = null)
        {
            var query = new Dictionary<string, string>();
            if (limit != null)
                query["limit"] = limit + string.Empty;
            if (offset != null)
                query["offset"] = offset + string.Empty;
            if (intents != null)
                query["intents"] = intents + string.Empty;
            var rsp = (await Request("/utterances", query: query)).Array;
            var list = rsp.Select(x => x.ToObject<Utterance>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns list of all traits associated with your app.
        /// </summary>
        public async Task<Trait[]> ListTraits()
        {
            var rsp = (await Request("/traits")).Array;
            var list = rsp.Select(x => x.ToObject<Trait>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns an array of all tag groups for an app.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        public async Task<AppVersion[]> ListAppVersions(string appId)
        {
            var endpoint = $"/apps/{WitHttp.Encode(appId)}/tags";
            var rsp = (await Request(endpoint)).Array;
            var list = rsp.Select(x => x.ToObject<AppVersion>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns names of all intents associated with your app.
        /// </summary>
        public async Task<Intent[]> ListIntents()
        {
            var rsp = (await Request("/intents")).Array;
            var list = rsp.Select(x => x.ToObject<Intent>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns list of all voices associated with your app.
        /// </summary>
        public async Task<Dictionary<string, Voice[]>> ListVoices()
        {
            var rsp = await Request("/voices");
            var item = rsp.Single.ToObject<Dictionary<string, Voice[]>>();
            return item;
        }

        /// <summary>
        /// Returns list of all entities associated with your app.
        /// </summary>
        public async Task<Entity[]> ListEntities()
        {
            var rsp = (await Request("/entities")).Array;
            var list = rsp.Select(x => x.ToObject<Entity>());
            return list.ToArray();
        }

        private static Task<string> ToJson(IMeaning m)
            => Task.FromResult(WitJson.Serialize(m));

        /// <summary>
        /// Runs interactive command line chat between user and bot. 
        /// Runs indefinitely until EOF is entered to the prompt. 
        /// </summary>
        /// <param name="handler">optional function to customize your response</param>
        /// <param name="ctx">optional initial context</param>
        /// <param name="cmd">optional command line reader</param>
        public async Task DoInteractive(WitCallback handler = null,
            IDictionary<string, string> ctx = null, CommandLine cmd = null)
        {
            handler ??= ToJson;
            cmd ??= new CommandLine();
            while (cmd.Prompt(InteractivePrompt) is { } msg)
            {
                var res = await GetMeaning(msg, ctx);
                var text = await handler(res);
                cmd.Print(text);
            }
        }

        public async Task DoInteractive(WitSyncCallback handler,
            IDictionary<string, string> ctx = null, CommandLine cmd = null)
            => await DoInteractive(m => Task.FromResult(handler.Invoke(m)), ctx, cmd);
    }
}