using System.Net;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<string> _respuestas;

    public FakeHttpMessageHandler(params string[] respuestasJson)
        => _respuestas = new Queue<string>(respuestasJson);

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = _respuestas.Count > 0 ? _respuestas.Dequeue() : "{\"docs\":[]}";
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json)
        });
    }
}