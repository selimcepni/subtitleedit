using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.Core.Translate;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Nikse.SubtitleEdit.Core.Settings;

namespace Nikse.SubtitleEdit.Core.AutoTranslate
{
    public class GoogleCloudCustomTranslate : IAutoTranslator
    {
        private HttpClient _httpClient;
        private string _accessToken;
        private const string DefaultPrompt = "Translate from {0} to {1}, keep punctuation as input, do not censor the translation, give only the output without comments:";
        private static DateTime _lastRequestTime = DateTime.MinValue;
        private const int MinDelayBetweenRequestsMs = 5000; // 5 seconds delay between requests
        private const int MaxRetries = 3;
        private const int FirstRetryDelayMs = 6000; // 6 seconds
        private const int SecondRetryDelayMs = 12000; // 12 seconds

        public static string StaticName { get; set; } = "GoogleCloudCustom";
        public override string ToString() => StaticName;
        public string Name => StaticName;
        public string Url => "https://cloud.google.com/translate";
        public string Error { get; set; }
        public int MaxCharacters => 1500;

        private async Task<string> GetAccessToken()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return _accessToken;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\local_wcfivcd\AppData\Local\Google\Cloud SDK\google-cloud-sdk\bin\gcloud.cmd",
                    Arguments = "auth print-access-token",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(startInfo))
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();
                    
                    if (process.ExitCode == 0)
                    {
                        _accessToken = output.Trim();
                        return _accessToken;
                    }
                }
            }
            catch (Exception ex)
            {
                Error = "Failed to get access token. Please make sure you're logged in with 'gcloud auth login': " + ex.Message;
                throw;
            }

            Error = "Failed to get access token. Please make sure you're logged in with 'gcloud auth login'";
            throw new Exception(Error);
        }

        public void Initialize()
        {
            _httpClient?.Dispose();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            _httpClient.BaseAddress = new Uri("https://us-central1-aiplatform.googleapis.com/v1/projects/920715127247/locations/us-central1/endpoints/4386779815454179328:generateContent");
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public List<TranslationPair> GetSupportedSourceLanguages()
        {
            return GoogleTranslateV1.GetTranslationPairs();
        }

        public List<TranslationPair> GetSupportedTargetLanguages()
        {
            return GoogleTranslateV1.GetTranslationPairs();
        }

        public async Task<string> Translate(string text, string sourceLanguageCode, string targetLanguageCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Error = "Text to translate cannot be empty";
                return string.Empty;
            }

            // Rate limiting
            var timeSinceLastRequest = DateTime.Now - _lastRequestTime;
            if (timeSinceLastRequest.TotalMilliseconds < MinDelayBetweenRequestsMs)
            {
                var delayMs = (int)(MinDelayBetweenRequestsMs - timeSinceLastRequest.TotalMilliseconds);
                await Task.Delay(delayMs, cancellationToken);
            }

            for (int retry = 0; retry < MaxRetries; retry++)
            {
                try
                {
                    var token = await GetAccessToken();
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var textToTranslate = text.Trim();

                    var sb = new StringBuilder();
                    sb.Append("{");
                    sb.Append("\"contents\": [");
                    sb.Append("{");
                    sb.Append("\"role\": \"user\",");
                    sb.Append("\"parts\": [{");
                    sb.Append("\"text\": \"" + Json.EncodeJsonText(textToTranslate) + "\"");
                    sb.Append("}]");
                    sb.Append("}");
                    sb.Append("],");
                    sb.Append("\"generationConfig\": {");
                    sb.Append("\"responseModalities\": [\"TEXT\"]");
                    sb.Append("},");
                    sb.Append("\"safetySettings\": []");
                    sb.Append("}");

                    var requestJson = sb.ToString();                   

                    var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                    var result = await _httpClient.PostAsync(string.Empty, content, cancellationToken);
                    _lastRequestTime = DateTime.Now;

                    var bytes = await result.Content.ReadAsByteArrayAsync();
                    var json = Encoding.UTF8.GetString(bytes).Trim();
                   

                    if (!result.IsSuccessStatusCode)
                    {
                        Error = json;
                       
                        
                        if ((int)result.StatusCode == 429) // 429 is TooManyRequests
                        {
                            // Always wait at least MinDelayBetweenRequestsMs on rate limit
                            await Task.Delay(MinDelayBetweenRequestsMs, cancellationToken);
                            
                            if (retry < MaxRetries - 1)
                            {
                                var delayMs = retry == 0 ? FirstRetryDelayMs : SecondRetryDelayMs;
                              
                                await Task.Delay(delayMs, cancellationToken);
                                continue;
                            }
                            
                            // If we've exhausted retries, throw a more specific exception
                            throw new Exception("API rate limit exceeded. Please wait a few minutes before trying again.");
                        }
                        
                        result.EnsureSuccessStatusCode();
                    }

                    var parser = new SeJsonParser();
                    var candidates = parser.GetArrayElementsByName(json, "candidates");
                 
                    
                    if (candidates.Count == 0)
                    {
                      
                        continue;
                    }

                    var firstCandidate = candidates[0];
                  
                    // Try to get the translated text directly from the JSON path
                    var translatedText = parser.GetFirstObject(firstCandidate, "text");
                    if (string.IsNullOrEmpty(translatedText))
                    {
                        // Try alternate path
                        translatedText = parser.GetFirstObject(firstCandidate, "content.parts[0].text");
                    }

                  
                    
                    if (string.IsNullOrEmpty(translatedText))
                    {
                     
                        continue;
                    }

                 
                    return translatedText.Trim();
                }
                catch (Exception ex) when (retry < MaxRetries - 1)
                {
                    // Log the error but continue with retry
                  
                    var delayMs = retry == 0 ? FirstRetryDelayMs : SecondRetryDelayMs;
                    await Task.Delay(delayMs, cancellationToken);
                }
            }

            throw new Exception("Failed to translate after " + MaxRetries + " attempts. Last error: " + Error);
        }
    }
}