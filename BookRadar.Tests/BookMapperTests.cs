
using BookRadar.App;

namespace BookRadar.Tests;

public class BookMapperTests
{
    [Fact]
    public void ToBook_MapeaLos3Campos_CuandoDocEsValido()
    {
        var doc = new BookDoc{Title = "Mistborn", AuthorName = new List<string> { "Brandon Sanderson"}, Key = "/works/0L160441242W"};
        var book = BookMapper.ToBook(doc);

        Assert.Equal("Mistborn", book.Title);
        Assert.Equal("Brandon Sanderson", book.Author);
        Assert.Equal("/works/0L160441242W", book.OpenLibraryKey);
    }
    [Fact]
    public void ToBook_DevuelveAutorDesconocido_CuandoAuthorNameEsNull()
    {
        var doc = new BookDoc{AuthorName = null};
        var book = BookMapper.ToBook(doc);

        Assert.Equal("Autor desconocido", book.Author);
    }

    [Fact]
    public void ToBook_UneLosAutoresPorComa_CuandoHayVarios()
    {
        var doc = new BookDoc{AuthorName = new List<string> {"Brandon Sanderson", "Robert Jordan"}};
        var book = BookMapper.ToBook(doc);

        Assert.Equal("Brandon Sanderson, Robert Jordan", book.Author);
    }
    [Fact]
    public void ToBook_DevuelveAutorDesconocido_CuandoAuthorNameEstaVacia()
    {
        var doc = new BookDoc { AuthorName = new List<string>() };  
        var book = BookMapper.ToBook(doc);

        Assert.Equal("Autor desconocido", book.Author);
    }
}
