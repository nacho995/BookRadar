using BookRadar.App;
using System.Text.Json;
using var db = new AppDbContext();
using var http = new HttpClient();

var json = await http.GetStringAsync("https://openlibrary.org/search.json?title=mistborn&limit=1");
// Console.WriteLine(json);
/*var books = new Book { Title = "Mistborn", Author = "Brandon Sanderson" };
db.Books.Add(books);
db.SaveChanges();
var booksDB = db.Books.ToList();
foreach(var book in booksDB)
{
    Console.WriteLine($"Me encanta {book.Title}, de {book.Author}");
}*/
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var response = JsonSerializer.Deserialize<OpenLibraryDTO>(json, options);


