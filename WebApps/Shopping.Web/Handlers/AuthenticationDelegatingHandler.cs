using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

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
        if (httpContext != null)
        {
            try
            {
                var accessToken = await httpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    _logger.LogDebug("Adding access token to request header");
                    request.Headers.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    _logger.LogWarning("No access token found in the current context");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving access token");
            }
        }
        else
        {
            _logger.LogWarning("HttpContext is null - cannot retrieve access token");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}