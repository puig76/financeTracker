using System;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FinanceTracker.Web.Clients;

public class AuthTokenHandler(IAccessTokenProvider tokenProvider) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenProvider.GetAccessTokenAsync();

        if (token is not null)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

