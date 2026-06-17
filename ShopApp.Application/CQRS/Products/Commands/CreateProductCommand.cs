using MediatR;
using ShopApp.Application.DTOs.Products;

namespace ShopApp.Application.CQRS.Products.Commands;

public record CreateProductCommand(CreateProductDto createProductDto) : IRequest<ProductDto>;