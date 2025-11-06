using System.Net.Http.Json;
using System.Text.Json;
using MyApp.WebApp.Clients.Models.Requests;

namespace MyApp.WebApp.Clients;

public class IdentityApiClient : IIdentityApiClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public IdentityApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/identity/register", request, SerializerOptions, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException($"Registration failed: {response.StatusCode}. {message}");
    }
}
