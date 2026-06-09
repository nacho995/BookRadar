namespace BookRadar.App;

public class EmbeddingResponse
{
    public EmbeddingData? Embedding { get; set; }
}
public class EmbeddingData
{
    public List<float>? Values { get; set; }
}
