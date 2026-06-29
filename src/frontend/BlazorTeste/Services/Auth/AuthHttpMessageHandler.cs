using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace BlazorTeste.Services.Auth;

public class AuthHttpMessageHandler(TokenService tokenService) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(tokenService.AccessToken))
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenService.AccessToken);

        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return base.SendAsync(request, cancellationToken);
    }
}
