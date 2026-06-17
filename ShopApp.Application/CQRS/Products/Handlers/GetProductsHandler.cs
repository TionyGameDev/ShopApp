using MediatR;
using ShopApp.Application.CQRS.Products.Queries;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Mappers;
using ShopApp.Domain.Entites;

namespace ShopApp.Application.CQRS.Products.Handlers;

public class GetProductsHandler(IProductRepository repository, ICacheService cache)
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync<List<Product>>("products");
        if (cached != null)
            return cached.Select(ProductMapper.ToDto).ToList();

        var products = await repository.GetAllAsync();
        await cache.SetAsync("products", products, TimeSpan.FromDays(1));
        return products.Select(ProductMapper.ToDto).ToList();
    }
}