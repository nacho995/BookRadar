using BookRadar.App;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Books")));
builder.Services.AddScoped<Recommender>();

var app = builder.Build();

app.MapGet("/api/books", (AppDbContext db, string? search) =>
    db.Books
    .Where(b => b.EmbeddingJson != null &&
            (search == null || b.Title!.ToLower().Contains(search.ToLower())))
    .Take(20)
    .Select(b => new { b.Id, b.Title, b.Author })
    .ToList());
app.MapGet("/api/books/{id}/recommendations", (AppDbContext db, Recommender recommender, Guid id) =>
{
    var book = db.Books.FirstOrDefault(b => b.Id == id);  
    if (book is null || book.EmbeddingJson is null) return Results.NotFound();
    var recs = recommender.Recommend(book);
    return Results.Ok(recs.Select(b => new {b.Id, b.Title, b.Author}));
});

app.Run();