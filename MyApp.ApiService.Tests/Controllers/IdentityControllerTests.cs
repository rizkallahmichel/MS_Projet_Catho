using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MyApp.ApiService.Contracts.Identity;
using MyApp.ApiService.Controllers;
using MyApp.Application.Users.Commands.RegisterClient;

namespace MyApp.ApiService.Tests.Controllers;

public class IdentityControllerTests
{
    private readonly Mock<IMediator> _mediator = new();

    private IdentityController CreateController() => new(_mediator.Object);

    [Fact]
    public async Task RegisterClient_ReturnsCreated()
    {
        var request = new RegisterClientRequest
        {
            Username = "john.doe",
            Email = "john.doe@example.com",
            Password = "Password123!",
            FirstName = "John",
            LastName = "Doe"
        };

        var expectedId = Guid.NewGuid().ToString();

        _mediator
            .Setup(m => m.Send(It.IsAny<RegisterClientCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RegisterClientResult(expectedId));

        var controller = CreateController();

        var result = await controller.RegisterClient(request, CancellationToken.None);

        var created = Assert.IsType<CreatedResult>(result.Result);
        var value = created.Value!;
        var idProperty = value.GetType().GetProperty("id");
        Assert.NotNull(idProperty);
        Assert.Equal(expectedId, idProperty!.GetValue(value));

        _mediator.Verify(m => m.Send(
                It.Is<RegisterClientCommand>(command =>
                    command.Username == request.Username &&
                    command.Email == request.Email &&
                    command.Password == request.Password &&
                    command.FirstName == request.FirstName &&
                    command.LastName == request.LastName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
