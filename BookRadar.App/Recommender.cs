using System.Text.Json;

namespace BookRadar.App;

public class Recommender
{
    private readonly AppDbContext _db;
    public Recommender(AppDbContext db) => _db = db;

    // Devuelve cada recomendación CON su score de similitud (para la API/UI)
    public List<(Book Book, double Score)> RecommendWithScores(Book referencia, int topN = 5)
    {
        var refVector = JsonSerializer.Deserialize<float[]>(referencia.EmbeddingJson!);
        var candidatos = _db.Books.Where(b => b.EmbeddingJson != null && b.Id != referencia.Id).ToList();

        return candidatos
            .Select(b => (Book: b, Score: CosineSimilarity(refVector!, JsonSerializer.Deserialize<float[]>(b.EmbeddingJson!)!)))
            .OrderByDescending(x => x.Score)
            .Take(topN)
            .ToList();
    }

    // La versión sin scores delega en la de arriba (la console app sigue funcionando igual)
    public List<Book> Recommend(Book referencia, int topN = 5)
        => RecommendWithScores(referencia, topN).Select(x => x.Book).ToList();
    public static double CosineSimilarity(float[] a, float[] b)
    {
        double dot = 0;
        double normA = 0;
        double normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot+= a[i] * b[i];
            normA+= a[i] * a[i];
            normB+= b[i] * b[i];
        }
        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        
    }
}