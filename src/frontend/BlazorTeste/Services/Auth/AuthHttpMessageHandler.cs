using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlazorTeste.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace BlazorTeste.Services.Auth;

public class AuthHttpMessageHandler(TokenService tokenService, Uri apiBaseAddress) : DelegatingHandler
{
    private readonly HttpClient _authClient = new() { BaseAddress = apiBaseAddress };
    private int _refreshing = 0;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var retryClone = await CloneRequestAsync(request);

        if (!string.IsNullOrEmpty(tokenService.AccessToken))
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenService.AccessToken);

        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        if (Interlocked.CompareExchange(ref _refreshing, 1, 0) != 0)
            return response;

        try
        {
            var refreshReq = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");
            refreshReq.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var refreshRes = await _authClient.SendAsync(refreshReq, cancellationToken);

            if (!refreshRes.IsSuccessStatusCode)
            {
                tokenService.ClearToken();
                return response;
            }

            var result = await refreshRes.Content.ReadFromJsonAsync<LoginResponse>(
                cancellationToken: cancellationToken);

            if (result is null)
            {
                tokenService.ClearToken();
                return response;
            }

            tokenService.SetToken(result.AccessToken);

            retryClone.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", result.AccessToken);
            retryClone.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            return await base.SendAsync(retryClone, cancellationToken);
        }
        finally
        {
            Interlocked.Exchange(ref _refreshing, 0);
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri) { Version = req.Version };

        foreach (var (k, v) in req.Headers)
            clone.Headers.TryAddWithoutValidation(k, v);

        foreach (var (k, v) in req.Options)
            clone.Options.TryAdd(k, v);

        if (req.Content is not null)
        {
            var bytes = await req.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var (k, v) in req.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(k, v);
        }

        return clone;
    }
}
