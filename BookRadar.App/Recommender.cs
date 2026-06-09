using System.Text.Json;

namespace BookRadar.App;

public class Recommender
{
    private readonly AppDbContext _db;
    public Recommender(AppDbContext db) => _db = db;

    public List<Book> Recommend(Book referencia, int topN = 5)
    {
        var refVector = JsonSerializer.Deserialize<float[]>(referencia.EmbeddingJson!);
        var candidatos = _db.Books.Where(b => b.EmbeddingJson != null && b.Id != referencia.Id).ToList();
        
        return candidatos
            .OrderByDescending(b => CosineSimilarity(refVector!, JsonSerializer.Deserialize<float[]>(b.EmbeddingJson!)!))
            .Take(topN)
            .ToList();
    }
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