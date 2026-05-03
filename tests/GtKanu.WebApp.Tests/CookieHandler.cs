namespace GtKanu.WebApp.Tests;

using System.Net;

internal sealed class CookieHandler : DelegatingHandler
{
    private readonly CookieContainer _container = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var cookieHeader = _container.GetCookieHeader(request.RequestUri!);
        if (!string.IsNullOrEmpty(cookieHeader))
        {
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var c in cookies)
            {
                _container.SetCookies(request.RequestUri!, c);
            }
        }
        return response;
    }
}
