using System;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LondonDataServices.IDecide.Manage.Server.Services;

    public interface ISecureTokenStorage
    {
        void StoreAccessToken(HttpContext context, string token, int expiresInSeconds);
        void StoreRefreshToken(HttpContext context, string token, int expiresInSeconds);
        string GetAccessToken(HttpContext context);
        string GetRefreshToken(HttpContext context);
        void ClearTokens(HttpContext context);
        void StoreCSRFState(HttpContext httpContext, string csrfState);
        string GetCSRFState(HttpContext httpContext);
        void ClearCSRFState(HttpContext httpContext);
    }

    public class SecureTokenStorage : ISecureTokenStorage
    {
        private readonly IDataProtector _protector;
        private readonly ILogger<SecureTokenStorage> _logger;

        private const string AccessTokenKey = "secure_access_token";
        private const string AccessTokenExpiresKey = "access_token_expires_at";
        private const string RefreshTokenKey = "secure_refresh_token";
        private const string RefreshTokenExpiresKey = "refresh_token_expires_at";
        private const string CRFSStateKey = "CRFS_State";

        public SecureTokenStorage(
            IDataProtectionProvider dataProtectionProvider,
            ILogger<SecureTokenStorage> logger)
        {
            // Create a dedicated protector for OAuth tokens
            _protector = dataProtectionProvider.CreateProtector("OAuthTokenProtection");
            _logger = logger;
        }

    public void StoreAccessToken(HttpContext context, string token, int expiresInSeconds)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Attempted to store null or empty access token");
            return;
        }

        try
        {
            // Encrypt token before storing
            var encryptedToken = _protector.Protect(Encoding.UTF8.GetBytes(token));
            var base64Token = Convert.ToBase64String(encryptedToken);

            context.Session.SetString(AccessTokenKey, base64Token);
            context.Session.SetString(AccessTokenExpiresKey,
                DateTime.UtcNow.AddSeconds(expiresInSeconds).ToString("O")); // ISO 8601 format

            _logger.LogDebug("Access token securely stored (expires in {Seconds}s)", expiresInSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt and store access token");
            throw;
        }
    }

    public void StoreRefreshToken(HttpContext context, string token, int expiresInSeconds)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Attempted to store null or empty refresh token");
            return;
        }

        try
        {
            var encryptedToken = _protector.Protect(Encoding.UTF8.GetBytes(token));
            var base64Token = Convert.ToBase64String(encryptedToken);

            context.Session.SetString(RefreshTokenKey, base64Token);
            context.Session.SetString(RefreshTokenExpiresKey,
                DateTime.UtcNow.AddSeconds(expiresInSeconds).ToString("O"));

            _logger.LogDebug("Refresh token securely stored (expires in {Seconds}s)", expiresInSeconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt and store refresh token");
            throw;
        }
    }

    public string GetAccessToken(HttpContext context)
    {
        try
        {
            var encryptedBase64 = context.Session.GetString(AccessTokenKey);
            if (string.IsNullOrEmpty(encryptedBase64))
            {
                return null;
            }

            // Check expiration
            var expiresAtStr = context.Session.GetString(AccessTokenExpiresKey);
            if (string.IsNullOrEmpty(expiresAtStr) ||
                !DateTime.TryParse(expiresAtStr, out var expiresAt) ||
                DateTime.UtcNow >= expiresAt)
            {
                _logger.LogDebug("Access token expired or invalid expiration");
                return null;
            }

            // Decrypt token
            var encryptedBytes = Convert.FromBase64String(encryptedBase64);
            var decryptedBytes = _protector.Unprotect(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt access token");
            return null;
        }
    }

    public string GetRefreshToken(HttpContext context)
    {
        try
        {
            var encryptedBase64 = context.Session.GetString(RefreshTokenKey);
            if (string.IsNullOrEmpty(encryptedBase64))
            {
                return null;
            }

            // Check expiration
            var expiresAtStr = context.Session.GetString(RefreshTokenExpiresKey);
            if (string.IsNullOrEmpty(expiresAtStr) ||
                !DateTime.TryParse(expiresAtStr, out var expiresAt) ||
                DateTime.UtcNow >= expiresAt)
            {
                _logger.LogDebug("Refresh token expired or invalid expiration");
                return null;
            }

            var encryptedBytes = Convert.FromBase64String(encryptedBase64);
            var decryptedBytes = _protector.Unprotect(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt refresh token");
            return null;
        }
    }

    public void ClearTokens(HttpContext context)
    {
        context.Session.Remove(AccessTokenKey);
        context.Session.Remove(AccessTokenExpiresKey);
        context.Session.Remove(RefreshTokenKey);
        context.Session.Remove(RefreshTokenExpiresKey);
        _logger.LogDebug("All tokens cleared from session");
    }

    public void StoreCSRFState(HttpContext context, string csrfState)
    {
        if (string.IsNullOrEmpty(csrfState))
        {
            _logger.LogWarning("Attempted to store null or empty state.");
            return;
        }

        try
        {
            var encryptedToken = _protector.Protect(Encoding.UTF8.GetBytes(csrfState));
            var base64Token = Convert.ToBase64String(encryptedToken);

            context.Session.SetString(CRFSStateKey, base64Token);

            _logger.LogDebug($"State securely stored: {csrfState}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to encrypt and store state.");
            throw;
        }
    }

    public string GetCSRFState(HttpContext context)
    {
        try
        {
            var encryptedBase64 = context.Session.GetString(CRFSStateKey);
            if (string.IsNullOrEmpty(encryptedBase64))
            {
                return null;
            }

            var encryptedBytes = Convert.FromBase64String(encryptedBase64);
            var decryptedBytes = _protector.Unprotect(encryptedBytes);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt refresh token");
            return null;
        }
    }

    public void ClearCSRFState(HttpContext context)
    {
        context.Session.Remove(CRFSStateKey);
        _logger.LogDebug("CRFS state cleared from session");
    }
}