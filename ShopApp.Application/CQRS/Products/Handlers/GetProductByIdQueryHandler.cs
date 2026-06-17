using MediatR;
using ShopApp.Application.CQRS.Products.Queries;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Mappers;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.CQRS.Products.Handlers;

public class GetProductByIdQueryHandler(IProductRepository repository, ICacheService cache) : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync<Product>(request.Id.ToString());
        if (cached != null)
            return cached.ToDto();

        var product = await repository.GetByIdAsync(request.Id);
        return product?.ToDto() ?? throw new NotFoundException("Product", request.Id);
    }
}