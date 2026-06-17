using MediatR;
using ShopApp.Application.DTOs.Products;

namespace ShopApp.Application.CQRS.Products.Commands;

public record UpdateProductCommand(ProductDto Dto) : IRequest<ProductDto>;