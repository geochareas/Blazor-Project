using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BlazorApp.Shared.Services;

public interface ITokenService
{
    Task<string> GetTokenAsync();
}

public class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private string? _accessToken;
    private DateTime _expiresAt;

    public TokenService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _expiresAt)
        {
            return _accessToken;
        }

        var tokenUrl = "https://localhost:7018/connect/token";

        // credentials should be stored in a secure store like Azure KeyVault,
        // hardcoding them just for the project

        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("client_id", "m2m"),
                new KeyValuePair<string, string>("client_secret", "secret"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("scope", "api")
            ])
        };

        var response = await _httpClient.SendAsync(request);   
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>();

        _accessToken = payload?.AccessToken;
        _expiresAt = DateTime.UtcNow.AddSeconds(payload?.ExpiresIn ?? 3600).AddMinutes(-1); // 1-minute early expiration

        return _accessToken!;
    }

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
