using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LondonDataServices.IDecide.Manage.Server.Services;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;
    private readonly ISecureTokenStorage _secureTokenStorage;

    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public TokenService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<TokenService> logger,
        ISecureTokenStorage secureTokenStorage)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _secureTokenStorage = secureTokenStorage;
        
        _tokenEndpoint = _configuration["CIS:TokenEndpoint"] ?? "";
        _clientId = _configuration["CIS:ClientId"] ?? "";
        _clientSecret = _configuration["CIS:ClientSecret"] ?? "";
    }

    public async Task<string> GetAccessTokenAsync(HttpContext httpContext)
    {
        var accessToken = _secureTokenStorage.GetAccessToken(httpContext);
        
        if (accessToken != null)
        {
            _logger.LogDebug("Valid access token retrieved from secure storage");
            return accessToken;
        }

        _logger.LogInformation("Access token expired or missing, attempting refresh");
        
        var refreshed = await RefreshAccessTokenAsync(httpContext);
        if (!refreshed)
        {
            _logger.LogWarning("Token refresh failed");
            return null;
        }

        return _secureTokenStorage.GetAccessToken(httpContext);
    }

    private async Task<bool> RefreshAccessTokenAsync(HttpContext httpContext)
    {
        var refreshToken = _secureTokenStorage.GetRefreshToken(httpContext);

        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("Refresh token not found in secure storage");
            await SignOutAsync(httpContext);
            return false;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.PostAsync(
                _tokenEndpoint,
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = refreshToken,
                    ["client_id"] = _clientId,
                    ["client_secret"] = _clientSecret
                })
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("CIS2 token refresh failed: {StatusCode} - {Error}", 
                    response.StatusCode, errorContent);
                await SignOutAsync(httpContext);
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();
            var parsedToken = JsonSerializer.Deserialize<TokenResult>(json);

            if (parsedToken == null || string.IsNullOrEmpty(parsedToken.AccessToken))
            {
                _logger.LogError("Failed to deserialize refresh token response");
                await SignOutAsync(httpContext);
                return false;
            }

            // ✅ Store refreshed tokens encrypted
            if (int.TryParse(parsedToken.ExpiresIn, out int accessExpires))
            {
                _secureTokenStorage.StoreAccessToken(httpContext, parsedToken.AccessToken, accessExpires);
            }

            if (!string.IsNullOrEmpty(parsedToken.RefreshToken) && 
                int.TryParse(parsedToken.RefreshTokenExpiresIn, out int refreshExpires))
            {
                _secureTokenStorage.StoreRefreshToken(httpContext, parsedToken.RefreshToken, refreshExpires);
            }

            _logger.LogInformation("Successfully refreshed access token");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing access token");
            await SignOutAsync(httpContext);
            return false;
        }
    }

    private async Task SignOutAsync(HttpContext httpContext)
    {
        _secureTokenStorage.ClearTokens(httpContext);
        httpContext.Session.Clear();
        await httpContext.SignOutAsync("bff-cookie");
    }
}