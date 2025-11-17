using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace PaperlessREST.OcrWorker.Services
{
    public class GenAiService
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly ILogger<GenAiService> _logger;

        public GenAiService(ILogger<GenAiService> logger)
        {
            _logger = logger;
            _http = new HttpClient();
            _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? throw new Exception("No GEMINI_API_KEY?");
        }

        public async Task<string> SummarizeAsync(string text)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new {
                        parts = new[]
                        {
                            new { text = $"The result of the ocr with tesseract is: {text}. Please respond only with a short summary consisting of 5 to 7 bullet points." }

                            // Gemini does not respond with ONLY the bulletpoints it NEEDS to also tell me the corrected text
                            // new { text = $"The result of the ocr with tesseract is: {text}. As you can see some words dont make sense. Could you please try to correct the content so it makes sense and afterwards create a short summary consisting of 5 to 7 bullet points? Finally tell me ONLY the summary bullet points and nothing else!" }
                        }
                    }
                }
            };

            _logger.LogInformation("Sending prompt {prompt} to Gemini.", requestBody.contents[0].parts[0].text);
            var response = await _http.PostAsJsonAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API summarization failed: {status}", response.StatusCode);
                return "";
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var result = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return result ?? "";
        }
    }
}
