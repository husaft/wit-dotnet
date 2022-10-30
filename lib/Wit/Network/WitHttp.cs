using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
                req.Headers.Add(pair.Key, pair.Value);
            return req;
        }

        public static JObject[] AsJsonObj(string text)
        {
            const string tmp = "}\r\n{";
            if (text.Contains(tmp))
            {
                var patch = $"[{text.Replace(tmp, "},{")}]";
                text = patch;
            }

            if (text.StartsWith("["))
            {
                var array = WitJson.Deserialize<JObject[]>(text);
                return array;
            }

            var json = WitJson.Deserialize<JObject>(text);
            return new[] { json };
        }

        public static string Encode(string input)
        {
            var text = HttpUtility.UrlEncode(input);
            return text;
        }
    }
}