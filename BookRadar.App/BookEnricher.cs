using System.Text.Json;

namespace BookRadar.App;

public class BookEnricher
{
    private readonly AppDbContext _db;
    private readonly OpenLibraryClient _client;
    public BookEnricher(AppDbContext db, OpenLibraryClient client)
    {
        _db = db;
        _client = client;
    }
    public async Task EnrichPendingAsync()
    {
        var pendientes = _db.Books.Where(b => b.Description == null && b.OpenLibraryKey != null).Take(2000).ToList();
        int n = 0;
        foreach (var book in pendientes)
        {
            n++;
            if (n % 50 == 0) { _db.SaveChanges(); Console.WriteLine($"  {n} enriquecidos ✓"); }
            var description = await _client.GetWorkDescriptionAsync(book.OpenLibraryKey!);
            if (description == null) { book.Description = ""; continue; }
            book.Description = description;
            book.EmbeddingJson = null;
            
        }
        _db.SaveChanges();
    }
}