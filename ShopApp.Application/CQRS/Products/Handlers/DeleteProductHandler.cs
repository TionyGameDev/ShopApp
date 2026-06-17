using MediatR;
using ShopApp.Application.CQRS.Products.Commands;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.CQRS.Products.Handlers;

public class DeleteProductHandler(IProductRepository repository, ICacheService cache) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id)
                      ?? throw new NotFoundException("Product", request.Id);

        await repository.RemoveAsync(product.Id);
        await cache.RemoveAsync("products");
        await cache.RemoveAsync(product.Id.ToString());
    }
}