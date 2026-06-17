using MediatR;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Services.ProductServices;

namespace ShopApp.Application.CQRS.Products.Queries;

public record GetProductsQuery : IRequest<List<ProductDto>>;