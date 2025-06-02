using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;

namespace Shopping.Web.Handlers;

public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthenticationDelegatingHandler> _logger;

    public AuthenticationDelegatingHandler(
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationDelegatingHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        _logger.LogDebug("AuthenticationDelegatingHandler invoked for {Uri}", request.RequestUri);

        if (httpContext != null)
        {
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                _logger.LogDebug("User is authenticated: {UserName}", httpContext.User.Identity.Name);

                try
                {
                    // Try to get the access token
                    var accessToken = await httpContext.GetTokenAsync("access_token");

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        _logger.LogInformation("Access token found, adding to request header");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        // Log first few characters of token for debugging
                        _logger.LogDebug("Token starts with: {TokenStart}...",
                            accessToken.Length > 20 ? accessToken.Substring(0, 20) : accessToken);
                    }
                    else
                    {
                        _logger.LogWarning("No access token found in the current context");

                        // Try alternative methods to get the token
                        var authResult = await httpContext.AuthenticateAsync();
                        if (authResult.Succeeded && authResult.Properties?.GetTokenValue("access_token") != null)
                        {
                            accessToken = authResult.Properties.GetTokenValue("access_token");
                            _logger.LogInformation("Access token retrieved from authentication result");
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        }
                        else
                        {
                            // Log available tokens for debugging
                            if (authResult.Properties?.GetTokens() != null)
                            {
                                foreach (var token in authResult.Properties.GetTokens())
                                {
                                    _logger.LogDebug("Available token: {TokenName}", token.Name);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving access token");
                }
            }
            else
            {
                _logger.LogWarning("User is not authenticated");
            }
        }
        else
        {
            _logger.LogWarning("HttpContext is null");
        }

        // Log the authorization header status
        if (request.Headers.Authorization != null)
        {
            _logger.LogInformation("Request has Authorization header: {Scheme}",
                request.Headers.Authorization.Scheme);
        }
        else
        {
            _logger.LogWarning("No Authorization header added to request");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}