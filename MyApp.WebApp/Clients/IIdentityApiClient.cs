using MyApp.WebApp.Clients.Models.Requests;

namespace MyApp.WebApp.Clients;

public interface IIdentityApiClient
{
    Task RegisterClientAsync(RegisterClientRequest request, CancellationToken cancellationToken = default);
}
