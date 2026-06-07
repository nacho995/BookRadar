namespace BookRadar.App;

public static class BookMapper
{
    public static Book ToBook(BookDoc doc)
    {
        return new Book
        {
            Title = doc.Title,
            Author = doc.AuthorName is {Count: > 0} ? string.Join(", ", doc.AuthorName) : "Autor desconocido",
            OpenLibraryKey = doc.Key
        };
    }
}