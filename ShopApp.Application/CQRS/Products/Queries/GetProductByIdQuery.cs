using MediatR;
using ShopApp.Application.DTOs.Products;

namespace ShopApp.Application.CQRS.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;