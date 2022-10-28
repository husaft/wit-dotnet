﻿using System;
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
using Wit.Network;

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

        private async Task<JObject[]> Request(string path, HttpMethod method = null,
            IDictionary<string, string> query = null, IDictionary<string, string> headers = null,
            bool isApiCall = true)
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

            using var response = await _http.SendAsync(request);
            var text = await response.Content.ReadAsStringAsync();
            var result = WitHttp.AsJsonObj(text);
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
        public void SendMessage(string msg)
        {

















            throw new NotImplementedException();
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
    }
}