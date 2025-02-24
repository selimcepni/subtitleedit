using Nikse.SubtitleEdit.Core.Common;
using Nikse.SubtitleEdit.Core.Http;
using Nikse.SubtitleEdit.Core.Translate;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nikse.SubtitleEdit.Core.AutoTranslate
{
    /// <summary>
    /// Google translate via Google Cloud V3 API - see https://cloud.google.com/translate/docs/reference/rest/v3/projects.locations/translateText
    /// </summary>
    public class GoogleTranslateV3 : IAutoTranslator, IDisposable
    {
        private string _accessToken;
        private DateTime _tokenExpiration;
        private IDownloader _httpClient;
        private string _projectId;

        public static string StaticName { get; set; } = "Google Translate V3 API";
        public override string ToString() => StaticName;
        public string Name => StaticName;
        public string Url => "https://translate.google.com/";
        public string Error { get; set; }
        public int MaxCharacters => 1500;

        private RSA ImportPrivateKey(string privateKey)
        {
            try
            {
                using (var stringReader = new StringReader(privateKey))
                {
                    var pemReader = new PemReader(stringReader);
                    var keyPair = pemReader.ReadObject();
                    
                    if (keyPair is AsymmetricCipherKeyPair pair)
                    {
                        var rsaParams = DotNetUtilities.ToRSAParameters(pair.Private as RsaPrivateCrtKeyParameters);
                        var rsa = RSA.Create();
                        rsa.ImportParameters(rsaParams);
                        return rsa;
                    }
                    else if (keyPair is RsaPrivateCrtKeyParameters privateKeyParams)
                    {
                        var rsaParams = DotNetUtilities.ToRSAParameters(privateKeyParams);
                        var rsa = RSA.Create();
                        rsa.ImportParameters(rsaParams);
                        return rsa;
                    }
                    
                    throw new Exception("Invalid private key format");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error importing private key: " + ex.Message);
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
            {
                return _accessToken;
            }

            var credentialsPath = Path.Combine(Configuration.BaseDirectory, "googlecloudserviceaccount.json");
            SeLogger.Error($"[GoogleTranslateV3 Request Log] Looking for credentials at: {credentialsPath}");

            if (!File.Exists(credentialsPath))
            {
                credentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "googlecloudserviceaccount.json");
                SeLogger.Error($"[GoogleTranslateV3 Request Log] Looking for credentials at: {credentialsPath}");

                if (!File.Exists(credentialsPath))
                {
                    credentialsPath = "googlecloudserviceaccount.json";
                    SeLogger.Error($"[GoogleTranslateV3 Request Log] Looking for credentials at: {credentialsPath}");

                    if (!File.Exists(credentialsPath))
                    {
                        throw new Exception("Service account credentials file not found. Please place googlecloudserviceaccount.json in the application directory.");
                    }
                }
            }

            var credentials = File.ReadAllText(credentialsPath);
            var credentialsJson = JObject.Parse(credentials);
            
            if (!credentialsJson.ContainsKey("private_key") || !credentialsJson.ContainsKey("client_email") || !credentialsJson.ContainsKey("project_id"))
            {
                throw new Exception("Invalid service account credentials file. Missing private_key, client_email, or project_id.");
            }
            
            _projectId = credentialsJson["project_id"].Value<string>();
            var privateKey = credentialsJson["private_key"].Value<string>();
            var clientEmail = credentialsJson["client_email"].Value<string>();

            if (string.IsNullOrWhiteSpace(privateKey) || string.IsNullOrWhiteSpace(clientEmail) || string.IsNullOrWhiteSpace(_projectId))
            {
                throw new Exception("Required credentials are empty in the service account credentials file.");
            }

            SeLogger.Error($"[GoogleTranslateV3 Request Log] Using client email: {clientEmail}");

            var now = DateTime.UtcNow;
            var expiration = now.AddHours(1);

            var payload = new Dictionary<string, object>
            {
                { "iss", clientEmail },
                { "scope", "https://www.googleapis.com/auth/cloud-translation" },
                { "aud", "https://oauth2.googleapis.com/token" },
                { "iat", ((DateTimeOffset)now).ToUnixTimeSeconds() },
                { "exp", ((DateTimeOffset)expiration).ToUnixTimeSeconds() }
            };

            var rsa = ImportPrivateKey(privateKey);

            var header = new Dictionary<string, object>
            {
                { "alg", "RS256" },
                { "typ", "JWT" }
            };

            var headerJson = JsonConvert.SerializeObject(header);
            var payloadJson = JsonConvert.SerializeObject(payload);

            var headerBytes = Encoding.UTF8.GetBytes(headerJson);
            var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);

            var headerBase64 = Convert.ToBase64String(headerBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            var payloadBase64 = Convert.ToBase64String(payloadBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var dataToSign = $"{headerBase64}.{payloadBase64}";
            var signature = rsa.SignData(Encoding.UTF8.GetBytes(dataToSign), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var signatureBase64 = Convert.ToBase64String(signature).TrimEnd('=').Replace('+', '-').Replace('/', '_');

            var jwt = $"{headerBase64}.{payloadBase64}.{signatureBase64}";

            var tokenRequest = new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "assertion", jwt }
            };

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://oauth2.googleapis.com/token", 
                    new FormUrlEncodedContent(tokenRequest));
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get access token: {await response.Content.ReadAsStringAsync()}");
                }

                var tokenResponse = await response.Content.ReadAsStringAsync();
                var tokenJson = JObject.Parse(tokenResponse);
                _accessToken = tokenJson["access_token"].Value<string>();
                _tokenExpiration = DateTime.UtcNow.AddSeconds(3600);

                return _accessToken;
            }
        }

        public void Initialize()
        {
            _httpClient = DownloaderFactory.MakeHttpClient();
            _httpClient.BaseAddress = new Uri("https://translate.googleapis.com/v3/");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            var accessToken = await GetAccessTokenAsync();
            var requestUrl = $"projects/{_projectId}/locations/global:translateText";

            // Sabit glossary yolu
            var glossaryPath = $"projects/stable-ring-451207-n1/locations/global/glossaries/tr_es_glossary";

            var requestBody = new
            {
                contents = new[] { text },
                sourceLanguageCode,
                targetLanguageCode,
                mimeType = "text/plain",
                glossaryConfig = new
                {
                    glossary = glossaryPath,
                    ignoreCase = true
                }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            
            SeLogger.Error($"[GoogleTranslateV3 Request Log] Request URL: {_httpClient.BaseAddress}{requestUrl}");
            SeLogger.Error($"[GoogleTranslateV3 Request Log] Request Body: {jsonContent}");
            SeLogger.Error($"[GoogleTranslateV3 Request Log] Authorization: Bearer {accessToken.Substring(0, 10)}...");

            string content;
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var result = await _httpClient.PostAsync(requestUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                content = await result.Content.ReadAsStringAsync();
                SeLogger.Error($"[GoogleTranslateV3 Request Log] Response Status: {result.StatusCode}");
                SeLogger.Error($"[GoogleTranslateV3 Request Log] Response Content: {content}");

                if (!result.IsSuccessStatusCode)
                {
                    Error = content;
                    SeLogger.Error($"Error in {StaticName}.Translate: " + Error);

                    if ((int)result.StatusCode == 401)
                    {                   
                        throw new Exception("Authentication failed. Please check your service account credentials.");
                    }

                    throw new Exception($"An error occurred calling GT translate - status code: {result.StatusCode}");
                }
            }
            catch (WebException webException)
            {
                var message = string.Empty;
                if (webException.Message.Contains("(401) Unauthorized"))
                {
                    message = "Authentication failed. Please check your service account credentials.";
                }

                throw new Exception(message, webException);
            }

            var response = JObject.Parse(content);

            // Önce glossary çevirisini kontrol et
            var glossaryTranslations = response["glossaryTranslations"] as JArray;
            if (glossaryTranslations != null && glossaryTranslations.Count > 0)
            {
                SeLogger.Error($"[GoogleTranslateV3 Glossary Log] Glossary translation found for text: {text}");
                SeLogger.Error($"[GoogleTranslateV3 Glossary Log] Glossary translations count: {glossaryTranslations.Count}");
                SeLogger.Error($"[GoogleTranslateV3 Glossary Log] Using glossary path: {glossaryPath}");

                var translatedText = glossaryTranslations[0]["translatedText"].ToString();
                try
                {
                    translatedText = Regex.Unescape(translatedText);
                }
                catch
                {
                    translatedText = translatedText.Replace("\\n", "\n");
                }

                translatedText = string.Join(Environment.NewLine, translatedText.SplitToLines());
                translatedText = TranslationHelper.PostTranslate(translatedText, targetLanguageCode);

                SeLogger.Error($"[GoogleTranslateV3 Glossary Log] Final translated text using glossary: {translatedText}");
                return translatedText;
            }
            else
            {
                SeLogger.Error($"[GoogleTranslateV3 Glossary Log] No glossary translation found for text: {text}");
            }

            // Glossary çevirisi yoksa normal çeviriyi kullan
            var translations = response["translations"] as JArray;
            
            if (translations == null || translations.Count == 0)
            {
                throw new Exception("No translation received from Google Translate V3");
            }

            var normalTranslatedText = translations[0]["translatedText"].ToString();
            try
            {
                normalTranslatedText = Regex.Unescape(normalTranslatedText);
            }
            catch
            {
                normalTranslatedText = normalTranslatedText.Replace("\\n", "\n");
            }

            normalTranslatedText = string.Join(Environment.NewLine, normalTranslatedText.SplitToLines());
            normalTranslatedText = TranslationHelper.PostTranslate(normalTranslatedText, targetLanguageCode);

            return normalTranslatedText;
        }

        public void Dispose() => _httpClient?.Dispose();
    }
} 