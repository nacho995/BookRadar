namespace BookRadar.App;

public class BookImporter
{
    private readonly AppDbContext _db;

    public BookImporter(AppDbContext db)
    {
        _db = db;
    }

    public void Import(IEnumerable<BookDoc> docs)
    {
        if (docs is not null)
        {
            foreach (var doc in docs)
            {
                if (doc.AuthorName is null || doc.AuthorName.Count == 0) continue;
                var book = BookMapper.ToBook(doc);
                bool yaExiste = _db.Books.Any(x => x.OpenLibraryKey == doc.Key);
                if (!yaExiste) _db.Books.Add(book);
            }
            _db.SaveChanges();
        }
    }
}