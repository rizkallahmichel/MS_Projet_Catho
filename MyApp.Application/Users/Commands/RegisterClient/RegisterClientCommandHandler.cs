using System.ComponentModel.DataAnnotations;
using MediatR;
using MyApp.Application.Common.Interfaces;
using MyApp.Application.Common.Models;

namespace MyApp.Application.Users.Commands.RegisterClient;

public sealed class RegisterClientCommandHandler : IRequestHandler<RegisterClientCommand, RegisterClientResult>
{
    private readonly IIdentityProvisioningService _identityProvisioningService;

    public RegisterClientCommandHandler(IIdentityProvisioningService identityProvisioningService)
    {
        _identityProvisioningService = identityProvisioningService;
    }

    public async Task<RegisterClientResult> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        Validate(request);

        var userId = await _identityProvisioningService.ProvisionClientUserAsync(
            new CreateClientUserModel(
                request.Username.Trim(),
                request.Email.Trim(),
                request.Password,
                request.FirstName?.Trim(),
                request.LastName?.Trim()),
            cancellationToken);

        return new RegisterClientResult(userId);
    }

    private static void Validate(RegisterClientCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ValidationException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ValidationException("Email is required.");
        }

        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            throw new ValidationException("Email address is invalid.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            throw new ValidationException("Password must be at least 8 characters long.");
        }
    }
}
