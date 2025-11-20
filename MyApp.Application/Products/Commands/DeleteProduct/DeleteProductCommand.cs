using MediatR;

namespace MyApp.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid PharmacyId, Guid ProductId) : IRequest;
