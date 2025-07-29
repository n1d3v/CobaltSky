using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.IO;
using System;

namespace CobaltSky.Classes
{
    class API
    {
        public string BskyAPIEndpoint = "https://bsky.social/xrpc";
        private static readonly HttpClient client = new HttpClient();

        public async Task SendAPI(string atMethod, string httpMethod, object jsonObj, Action<string> callback, Dictionary<string, string> Headers = null)
        {
            try
            {
                // Define what a request is
                var request = new HttpRequestMessage(new HttpMethod(httpMethod.ToUpperInvariant()), BskyAPIEndpoint + atMethod);

                // Add the headers from the constructor (if it isn't null)
                if (Headers != null)
                {
                    foreach (var header in Headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }

                // If it's a POST request, serialize the object and make it a string. (If it's null, don't use a body.)
                if (httpMethod.ToUpperInvariant() == "POST" && jsonObj != null)
                {
                    string jsonBody = SerializeToJson(jsonObj);
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("[API] Request is successful!");
                    callback(responseBody);
                }
                else
                {
                    callback($"[API] Error: {response.StatusCode} | {responseBody}");
                }
            }
            catch (Exception ex)
            {
                callback($"[API] Error: {ex.Message}");
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