using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CobaltSky.Classes
{
    class API
    {
        public readonly string APIEndpoint = "https://bsky.social/xrpc";

        private static readonly HttpClient client = new HttpClient();

        public async Task APISend(object jsonObject, Action<string> callback, string atMethod, Dictionary<string, string> Headers = null, string httpMethod = "POST")
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(httpMethod.ToUpperInvariant()), APIEndpoint + atMethod);

                if (Headers != null)
                {
                    foreach (var header in Headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                if (httpMethod.ToUpperInvariant() == "POST")
                {
                    string jsonBody = SerializeToJson(jsonObject);
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    callback(responseBody);
                }
                else
                {
                    callback($"API error: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                callback("API error: " + ex.Message);
            }
        }

        private string SerializeToJson(object obj)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
            }
        }
    }
}