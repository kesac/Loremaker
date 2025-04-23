using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Loremaker.Completions.OpenRouter
{
    /// <summary>
    /// A client for accessing the OpenRouter completions API.
    /// </summary>
    public class OpenRouterClient
    {
        public static readonly string CompletionsApi = "https://openrouter.ai/api/v1/chat/completions";
        public static readonly string ModelsApi = "https://openrouter.ai/api/v1/models";

        /// <summary>
        /// The HTTP client used to make requests to the OpenRouter API.
        /// It's recommended in the dotnet docs to use long-lived instances of HttpClient
        /// or else there is risk in running out of sockets.
        /// </summary>
        private HttpClient _httpClient;
        private string _apiKey;

        /// <summary>
        /// Initializes a new client for the OpenRouter API 
        /// and uses the API key specified in the <c>LOREMAKER_OPENROUTER_KEY</c>
        /// environmental variable for future requests.
        /// The environmental variable must be set under the user executing the code.
        /// </summary>
        public OpenRouterClient()
        {
            _httpClient = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("LOREMAKER_OPENROUTER_KEY", EnvironmentVariableTarget.User);

            // The API key is not accepted as a constructor argument to
            // avoid accidental exposure of the key in source control.
        }

        /// <summary>
        /// <para>
        ///     <em>No authentication required.</em>
        ///     Accesses the Models API and gets information of all AI models currently 
        ///     supported by OpenRouter including their IDs, descriptions, and prices. 
        /// </para>
        /// </summary>
        /// <param name="cachedJsonFile">
        /// <para>
        ///     If a path to a JSON file is provided, its contents will be used to create a 
        ///     <see cref="ModelsApiResponse"/> instead of calling the Models API.
        /// </para>
        /// <para>
        ///     If a file does not exist at the path, the API will be called and the JSON
        ///     file will be created as a cached copy of the response for subsequent calls.
        /// </para>
        /// <para>
        ///     Leaving this argument null will always force a call to the API.
        /// </para>
        /// </param>
        /// <exception cref="InvalidOperationException"/>
        public async Task<ModelsApiResponse> GetModels(string cachedJsonFile = null)
        {
            var result = new ModelsApiResponse();

            try {

                var json = string.Empty;

                if(cachedJsonFile != null && File.Exists(cachedJsonFile))
                {
                    json = File.ReadAllText(cachedJsonFile);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Clear();
                    var response = await _httpClient.GetAsync(ModelsApi);
                    response.EnsureSuccessStatusCode();
                    json = await response.Content.ReadAsStringAsync();
                }

                // Cached file path provided, but the file hasn't been created yet
                if (cachedJsonFile != null && !File.Exists(cachedJsonFile))
                {
                    File.WriteAllText(cachedJsonFile, json);
                }

                var deserialized = JsonSerializer.Deserialize<ModelsApiResponse>(json);
                result.Models = deserialized.Models;

            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not retrieve OpenRouter model information: " + e.Message, e);
            }

            return result;
        }

        /// <summary>
        /// <para>
        ///     <em>Authentication required.</em>
        ///     Sends a message to OpenRouter's completions API and
        ///     returns the response.
        /// </para>
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public async Task<CompletionsApiResponse> GetCompletion(string model, string message)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var request = new CompletionsApiRequest(model);
                request.AddMessage("user", message);
                var json = request.ToJson();

                var response = await _httpClient.PostAsync(CompletionsApi, new StringContent(json));
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<CompletionsApiResponse>(responseContent);

                return result;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not get response to completions request: " + e.Message, e);
            }
        }

    }


}
