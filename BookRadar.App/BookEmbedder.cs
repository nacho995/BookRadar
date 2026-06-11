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
        var pendientes = _db.Books.Where(b => b.EmbeddingJson == null).Take(500).ToList();
        int contador = 0;
        int fallos = 0;
        foreach (var book in pendientes)
        {
            contador++;
            if(contador % 25 == 0) _db.SaveChanges();
            var texto = $"{book.Title} {book.Description ?? ""} ";
            var vector = await _embeddingClient.GetEmbeddingAsync(texto);
            await Task.Delay(500);
            if (vector.Length == 0) 
            {
                fallos++;
                if (fallos >= 10) { Console.WriteLine("  ✋ Cuota agotada — abortando pasada"); break; }
                continue;
            }
            book.EmbeddingJson = JsonSerializer.Serialize(vector);
        }
        Console.WriteLine($"{contador} intentados, {fallos} fallos");
        _db.SaveChanges();
    }
}