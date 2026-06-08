using BookRadar.App;

namespace BookRadar.Tests;

public class FakeHttpMessageHandlerTests
{
    [Fact]
    public async Task SearchBySubject_AcumulaVariasPaginas_HastaLaVacia()
    {
        // Arrange: 3 respuestas en cola — pág1 (2 libros), pág2 (1 libro), pág3 (vacía)
        var pagina1 = """
          {"docs":[{"title":"Libro A","author_name":["Autor 1"],"key":"/works/OL1W"},
                   {"title":"Libro B","author_name":["Autor 2"],"key":"/works/OL2W"}]}
          """;
        var pagina2 = """
          {"docs":[{"title":"Libro C","author_name":["Autor 3"],"key":"/works/OL3W"}]}
          """;
        var paginaVacia = """{"docs":[]}""";

        var handler = new FakeHttpMessageHandler(pagina1, pagina2, paginaVacia);
        var http = new HttpClient(handler);
        var client = new OpenLibraryClient(http);

        var fantasyBooks = await client.SearchBySubjectAsync("fantasy");

        Assert.Equal(3, fantasyBooks.Count);
    }
}