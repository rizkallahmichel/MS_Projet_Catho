using MyApp.Application.Common.Models;

namespace MyApp.Application.Common.Interfaces;

public interface IIdentityProvisioningService
{
    Task<string> ProvisionPharmacyUserAsync(CreatePharmacyUserModel user, CancellationToken cancellationToken);
    Task<string> ProvisionClientUserAsync(CreateClientUserModel user, CancellationToken cancellationToken);
}
