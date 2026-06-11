using BookRadar.App;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Books")));
builder.Services.AddScoped<Recommender>();

var app = builder.Build();

app.UseDefaultFiles();   // en "/" sirve wwwroot/index.html
app.UseStaticFiles();    // sirve todo lo que haya en wwwroot/ (el frontend)

app.MapGet("/api/books", (AppDbContext db, string? search) =>
    db.Books
    .Where(b => b.EmbeddingJson != null &&
            (search == null || b.Title!.ToLower().Contains(search.ToLower())))
    .Take(20)
    .Select(b => new { b.Id, b.Title, b.Author, b.Description, b.OpenLibraryKey })  // EF: solo estas columnas viajan
    .AsEnumerable()                                                // de aquí en adelante, en memoria
    .Select(b => new { b.Id, b.Title, b.Author, Desc = Snip(b.Description), Key = b.OpenLibraryKey })
    .ToList());

app.MapGet("/api/books/{id}/recommendations", (AppDbContext db, Recommender recommender, Guid id) =>
{
    var book = db.Books.FirstOrDefault(b => b.Id == id);
    if (book is null || book.EmbeddingJson is null) return Results.NotFound();
    var recs = recommender.RecommendWithScores(book);
    return Results.Ok(recs.Select(x => new
    {
        x.Book.Id,
        x.Book.Title,
        x.Book.Author,
        Desc = Snip(x.Book.Description),
        Key = x.Book.OpenLibraryKey,
        Score = Math.Round(x.Score, 3)
    }));
});

// Recorta sinopsis largas para no inflar el payload (la UI solo muestra un extracto)
static string? Snip(string? s) =>
    string.IsNullOrEmpty(s) ? null : (s.Length > 220 ? s[..220] + "…" : s);

app.Run();