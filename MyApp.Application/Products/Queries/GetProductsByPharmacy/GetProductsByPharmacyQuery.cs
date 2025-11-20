using MediatR;
using MyApp.Application.Common.Models;

namespace MyApp.Application.Products.Queries.GetProductsByPharmacy;

public record GetProductsByPharmacyQuery(Guid PharmacyId, Guid? CategoryId) : IRequest<IReadOnlyList<ProductDto>>;
