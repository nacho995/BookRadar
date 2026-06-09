using BookRadar.App;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();
db.Database.Migrate();
using var http = new HttpClient();

// 1. Catálogo (asegúrate de tenerlo poblado — ver abajo)
var client = new OpenLibraryClient(http);
var importer = new BookImporter(db);
var generos = new[] { "fantasy", "fiction", "programming" };
foreach (var genero in generos)
    importer.Import(await client.SearchBySubjectAsync(genero));

// 2. Embeber los pendientes
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (string.IsNullOrEmpty(apiKey)) { Console.WriteLine("falta la key"); return; }
var embeddingClient = new EmbeddingClient(http, apiKey);
var embedder = new BookEmbedder(db, embeddingClient);
await embedder.EmbedPendingAsync();

Console.WriteLine($"{db.Books.Count(b => b.EmbeddingJson != null)} libros con embedding");

var recommender = new Recommender(db);
var referencia = db.Books.First(b => b.EmbeddingJson != null);
var recomendaciones = recommender.Recommend(referencia);

Console.WriteLine($"\nPorque te gustó '{referencia.Title}':");
foreach (var libro in recomendaciones)
    Console.WriteLine($"  - {libro.Title} ({libro.Author})");