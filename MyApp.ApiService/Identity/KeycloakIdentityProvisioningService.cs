using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyApp.Application.Common.Interfaces;
using MyApp.Application.Common.Models;

namespace MyApp.ApiService.Identity;

public sealed class KeycloakIdentityProvisioningService : IIdentityProvisioningService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _options;
    private readonly ILogger<KeycloakIdentityProvisioningService> _logger;

    public KeycloakIdentityProvisioningService(
        HttpClient httpClient,
        IOptions<KeycloakOptions> options,
        ILogger<KeycloakIdentityProvisioningService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> ProvisionPharmacyUserAsync(CreatePharmacyUserModel user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            throw new InvalidOperationException("Keycloak base URL is not configured.");
        }

        var adminToken = await GetAdminAccessTokenAsync(cancellationToken);
        var userId = await CreateUserAsync(adminToken, user, cancellationToken);
        await AssignRealmRoleAsync(adminToken, userId, _options.PharmacyRoleName, cancellationToken);

        return userId;
    }

    private async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken)
    {
        var adminRealm = string.IsNullOrWhiteSpace(_options.AdminRealm)
            ? _options.Realm
            : _options.AdminRealm;

        if (string.IsNullOrWhiteSpace(adminRealm))
        {
            throw new InvalidOperationException("Keycloak admin realm is not configured.");
        }

        var tokenEndpoint = $"realms/{adminRealm}/protocol/openid-connect/token";
        using var content = new FormUrlEncodedContent(BuildTokenRequestPayload());

        var response = await _httpClient.PostAsync(tokenEndpoint, content, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to acquire Keycloak admin token. Status: {response.StatusCode}. Response: {body}");
        }

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (!payload.TryGetProperty("access_token", out var accessToken))
        {
            throw new InvalidOperationException("Keycloak token response is missing access_token.");
        }

        return accessToken.GetString() ?? throw new InvalidOperationException("Keycloak access_token was null.");
    }

    private Dictionary<string, string> BuildTokenRequestPayload()
    {
        if (!string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            return new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            };
        }

        if (string.IsNullOrWhiteSpace(_options.Username) || string.IsNullOrWhiteSpace(_options.Password))
        {
            throw new InvalidOperationException("Keycloak admin credentials are not configured.");
        }

        return new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _options.ClientId,
            ["username"] = _options.Username,
            ["password"] = _options.Password
        };
    }

    private async Task<string> CreateUserAsync(string accessToken, CreatePharmacyUserModel user, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, $"admin/realms/{_options.Realm}/users");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var payload = new
        {
            username = user.Username,
            email = user.Email,
            enabled = true,
            emailVerified = true,
            firstName = user.FirstName,
            lastName = user.LastName,
            attributes = new Dictionary<string, object?>
            {
                ["pharmacyId"] = new[] { user.PharmacyId.ToString() }
            },
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = user.Password,
                    temporary = false
                }
            }
        };

        request.Content = JsonContent.Create(payload, options: SerializerOptions);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new InvalidOperationException("Keycloak user with the specified username or email already exists.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to create Keycloak user. Status: {response.StatusCode}. Response: {body}");
        }

        var userId = ExtractUserIdFromLocation(response.Headers.Location);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            return userId;
        }

        return await LookupUserIdAsync(accessToken, user.Username, cancellationToken);
    }

    private static string? ExtractUserIdFromLocation(Uri? location)
    {
        if (location is null)
        {
            return null;
        }

        var segments = location.Segments;
        if (segments.Length == 0)
        {
            return null;
        }

        return segments[^1].Trim('/');
    }

    private async Task<string> LookupUserIdAsync(string accessToken, string username, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"admin/realms/{_options.Realm}/users?username={Uri.EscapeDataString(username)}&exact=true");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
        if (payload.ValueKind != JsonValueKind.Array || payload.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Keycloak did not return the newly created user.");
        }

        var userElement = payload[0];
        if (!userElement.TryGetProperty("id", out var idElement))
        {
            throw new InvalidOperationException("Keycloak user representation is missing the id property.");
        }

        return idElement.GetString() ?? throw new InvalidOperationException("Keycloak user id was null.");
    }

    private async Task AssignRealmRoleAsync(string accessToken, string userId, string roleName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            _logger.LogWarning("Keycloak pharmacy role name is not configured. Skipping role assignment for user {UserId}", userId);
            return;
        }

        var role = await GetRealmRoleAsync(accessToken, roleName, cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(new[] { role }, SerializerOptions), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to assign Keycloak role '{roleName}' to user. Status: {response.StatusCode}. Response: {body}");
        }
    }

    private async Task<JsonElement> GetRealmRoleAsync(string accessToken, string roleName, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"admin/realms/{_options.Realm}/roles/{Uri.EscapeDataString(roleName)}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to fetch Keycloak role '{roleName}'. Status: {response.StatusCode}. Response: {body}");
        }

        return await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
    }
}
