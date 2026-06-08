using BookRadar.App;
using Microsoft.EntityFrameworkCore;

using var db = new AppDbContext();
db.Database.Migrate();
using var http = new HttpClient();
var client = new OpenLibraryClient(http);
var importer = new BookImporter(db);

var generos = new[] { "fantasy", "fiction", "programming" };

foreach(var genero in generos)
{
    var docs = await client.SearchBySubjectAsync(genero);
    importer.Import(docs);
}

Console.WriteLine($"{db.Books.Count()} books");

// Console.WriteLine(json);
/*var books = new Book { Title = "Mistborn", Author = "Brandon Sanderson" };
db.Books.Add(books);
db.SaveChanges();
var booksDB = db.Books.ToList();
foreach(var book in booksDB)
{
    Console.WriteLine($"Me encanta {book.Title}, de {book.Author}");
}*/



