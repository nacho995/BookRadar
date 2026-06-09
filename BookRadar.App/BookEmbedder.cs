using System.Text.Json;

namespace BookRadar.App;

public class BookEmbedder
{
    private readonly AppDbContext _db;
    private readonly EmbeddingClient _embeddingClient;
    public BookEmbedder(AppDbContext db, EmbeddingClient embeddingClient)
    {
        _db = db;
        _embeddingClient = embeddingClient;
    }
    public async Task EmbedPendingAsync()
    {
        var pendientes = _db.Books.Where(b => b.EmbeddingJson == null).Take(100).ToList();
        foreach (var book in pendientes)
        {
            var texto = $"{book.Title} {book.Author}";
            var vector = await _embeddingClient.GetEmbeddingAsync(texto);
            if (vector.Length == 0) continue;
            book.EmbeddingJson = JsonSerializer.Serialize(vector);

        }
        _db.SaveChanges();
    }
}