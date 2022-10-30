using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public Task<JObject[]> RequestExt(string path, IDictionary<string, string> query = null)
            => Request(path, query: query, isApiCall: false);

        public string WitApiHost { get; set; } = WitConstants.DefaultWitUrl;
        public string WitApiVersion { get; set; } = WitConstants.DefaultApiVersion;
        public string InteractivePrompt { get; set; } = "> ";

        private async Task<JObject[]> Request(string path, HttpMethod method = null,
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
            var text = await response.Content.ReadAsStringAsync();
            var result = AsJsonObj(text);
            var json = result.FirstOrDefault();

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
            var rsp = (await Request("/message", query: query)).Single();
            var item = rsp.ToObject<Meaning>();
            return item;
        }

        /// <summary>
        /// Returns the list of the top detected locales for the text message.
        /// </summary>
        /// <param name="msg">the text message</param>
        public async Task<DetectedLocale[]> DetectLanguage(string msg)
        {
            var query = new Dictionary<string, string> { ["q"] = msg };
            var res = await Request("/language", query: query);
            var rsp = res.First()["detected_locales"];
            return rsp?.ToObject<DetectedLocale[]>();
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
            var rsp = await Request("/apps", query: query);
            var list = rsp.Select(x => x.ToObject<AppInfo>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns an object representation of the specified app.
        /// </summary>
        /// <param name="appId">ID of existing app</param>
        public async Task<AppInfo> GetAppInfo(string appId)
        {
            var endpoint = $"/apps/{WitHttp.Encode(appId)}";
            var rsp = (await Request(endpoint)).Single();
            var item = rsp.ToObject<AppInfo>();
            return item;
        }

        /// <summary>
        /// Get a URL where you can download a ZIP file containing all of your app data.
        /// </summary>
        public async Task<Uri> GetExportUrl()
        {
            var rsp = (await Request("/export")).Single();
            var url = rsp["uri"]!.Value<string>();
            var uri = new Uri(url!);
            return uri;
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
            var rsp = await Request("/utterances", query: query);
            var list = rsp.Select(x => x.ToObject<Utterance>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns list of all traits associated with your app.
        /// </summary>
        public async Task<Trait[]> ListTraits()
        {
            var rsp = await Request("/traits");
            var list = rsp.Select(x => x.ToObject<Trait>());
            return list.ToArray();
        }

        /// <summary>
        /// Returns list of all entities associated with your app.
        /// </summary>
        public async Task<Entity[]> ListEntities()
        {
            var rsp = await Request("/entities");
            var list = rsp.Select(x => x.ToObject<Entity>());
            return list.ToArray();
        }

        private static Task<string> ToJson(Meaning m)
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