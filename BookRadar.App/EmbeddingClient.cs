using System.Text;
using System.Text.Json;

namespace BookRadar.App;

public class EmbeddingClient{
    
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public EmbeddingClient(HttpClient http, string apiKey)
    {
        _http = http;
        _apiKey = apiKey;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var body = new { content = new { parts = new[] { new {text } } } };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post,
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-2:embedContent");
        request.Headers.Add("x-goog-api-key", _apiKey);
        request.Content = content;

        var response = await _http.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseJson, options);

        return embeddingResponse?.Embedding?.Values?.ToArray() ?? Array.Empty<float>();
    }
}