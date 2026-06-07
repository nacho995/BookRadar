using BookRadar.App;

using Microsoft.Data.Sqlite;        // SqliteConnection
using Microsoft.EntityFrameworkCore; // DbContextOptionsBuilder, UseSqlite, EnsureCreated

namespace BookRadar.Tests;

public class BookImporterTest{
[Fact]
public void Import_NoDuplica_CuandoSeImportaElMismoLibroDosVeces()
{
    
    var doc = new BookDoc { Key = "/works/OL16044142W" };

        // --- Arrange: BBDD SQLite en memoria ---
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();   //  clave: ver abajo

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);  // usa el constructor (2) que añadiste
        context.Database.EnsureCreated(); // usa el constructor (2) que añadiste
        var importer = new BookImporter(context);
        importer.Import(new[] { doc });
        importer.Import(new[] { doc });

        Assert.Equal(1, context.Books.Count());
}
}