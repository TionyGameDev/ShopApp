using MediatR;
using ShopApp.Application.CQRS.Products.Commands;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Mappers;

namespace ShopApp.Application.CQRS.Products.Handlers;

public class CreateProductHandler(IProductRepository repository, ICacheService cache) : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var productNew = command.createProductDto.NewProduct();
        var product =  await repository.AddAsync(productNew);
        await cache.RemoveAsync("products");
        await cache.SetAsync(product.Id.ToString(),product);
        return product.ToDto();
    }
}