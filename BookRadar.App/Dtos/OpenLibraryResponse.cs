using System.Text.Json.Serialization;

namespace BookRadar.App;

public class OpenLibraryDTO
{
    public List<BookDoc>? Docs { get; set; }

}

public class BookDoc
{
    public string? Title { get; set;}
    [JsonPropertyName("author_name")]
    public List<string>? AuthorName { get; set; }
    public string? Key { get; set; }
}