using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Wit.Tools;

namespace Wit.Network
{
    public static class WitHttp
    {
        public static Uri AddQuery(this Uri uri,
            IDictionary<string, string> query)
        {
            var start = false;
            var sb = new StringBuilder();
            sb.Append(uri);
            if (query != null)
                foreach (var pair in query)
                {
                    if (pair.Value == null)
                        continue;
                    sb.Append(start ? '&' : '?');
                    sb.Append(pair.Key);
                    sb.Append('=');
                    sb.Append(pair.Value);
                    start = true;
                }
            var txt = sb.ToString();
            return new Uri(txt);
        }

        public static HttpRequestMessage AddHeaders(
            this HttpRequestMessage req,
            IDictionary<string, string> headers)
        {
            if (headers == null)
                return req;
            foreach (var pair in headers)
            {
                var key = pair.Key;
                var val = pair.Value;
                req.Headers.Add(key, val);
            }
            return req;
        }

        public static async Task<WitResult> ParseResult(HttpContent content)
        {
            var mediaType = content.Headers.ContentType?.MediaType;
            if (mediaType != "application/json")
            {
                var orig = await content.ReadAsStreamAsync();
                var stream = new MemoryStream();
                await orig.CopyToAsync(stream);
                stream.Position = 0L;
                return new WitResult(mediaType, null, null, stream);
            }

            var text = await content.ReadAsStringAsync();

            const string tmp = "}\r\n{";
            if (text.Contains(tmp))
            {
                var patch = $"[{text.Replace(tmp, "},{")}]";
                text = patch;
            }

            if (text.StartsWith("["))
            {
                var array = WitJson.Deserialize<JObject[]>(text);
                return new WitResult(mediaType, null, array, null);
            }

            var json = WitJson.Deserialize<JObject>(text);
            return new WitResult(mediaType, json, null, null);
        }

        public static string Encode(string input)
        {
            var text = HttpUtility.UrlEncode(input);
            return text;
        }
    }
}