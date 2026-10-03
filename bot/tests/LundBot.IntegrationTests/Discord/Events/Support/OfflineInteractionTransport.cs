using System.Net;

namespace LundBot.IntegrationTests.Discord.Events.Support;

internal sealed class OfflineInteractionTransport : HttpMessageHandler
{
    public List<string> Requests { get; } = [];
    public bool RejectRequests { get; set; }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        if (
            request.Method != HttpMethod.Post
            || request.RequestUri?.AbsolutePath.Contains("/interactions/", StringComparison.Ordinal) != true
        )
        {
            throw new InvalidOperationException(
                $"Unexpected Discord HTTP request: {request.Method} {request.RequestUri?.AbsolutePath}"
            );
        }
        Requests.Add(
            request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync(cancellationToken)
        );
        return new HttpResponseMessage(RejectRequests ? HttpStatusCode.BadRequest : HttpStatusCode.NoContent)
        {
            Content = new StringContent(
                RejectRequests ? """{"message":"Test rejection","code":50035}""" : string.Empty
            ),
        };
    }
}
