using System.Text.Json;
using Polly;
using Polly.Retry;
namespace BookRadar.App;

public class OpenLibraryClient
{
    private readonly HttpClient _http;
    private readonly ResiliencePipeline _pipeline;   // ← (1) campo nuevo, tipo de Polly

    public OpenLibraryClient(HttpClient http)
    {
        _http = http;
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    Console.WriteLine($"  ⟳ Reintento {args.AttemptNumber} tras fallo...");
                    return default;
                }
            })
            .Build();
    }

    public async Task<List<BookDoc>> SearchBySubjectAsync(string subject, int max = 500)
    {
        var resultados  = new List<BookDoc>();
        int page = 1;
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        while (resultados.Count < max){
            var url = $"https://openlibrary.org/search.json?subject={subject}&language=spa&limit=100&page={page}";
            var json = await _pipeline.ExecuteAsync(async ct => await _http.GetStringAsync(url, ct));
            var response = JsonSerializer.Deserialize<OpenLibraryDTO>(json, options);
            var docs = response?.Docs ?? new List<BookDoc>();
            if (docs.Count == 0) break;

            resultados.AddRange(docs);
            page++;
}
        return resultados.Take(max).ToList();
    }
    public async Task<string?> GetWorkDescriptionAsync(string key)  
    {
        // 1. TRAER — igual que en Search: URL + pipeline
        var url = $"https://openlibrary.org{key}.json";
        var json = await _pipeline.ExecuteAsync(async ct => await _http.GetStringAsync(url, ct));

        // 2. DESERIALIZAR — igual que siempre; DE AQUÍ SALE 'work'
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var work = JsonSerializer.Deserialize<WorkDto>(json, options);
        if (work is null) return null;

        // 3. EXTRAER — el switch que ya tenías, devolviendo directamente
        return work.Description.ValueKind switch
        {
            JsonValueKind.String => work.Description.GetString(),
            JsonValueKind.Object when work.Description.TryGetProperty("value", out var v) => v.GetString(),
            _ => null
        };
    }
}