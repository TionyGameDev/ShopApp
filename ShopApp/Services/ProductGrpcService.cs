using Grpc.Core;
using MediatR;
using ShopApp.Application.CQRS.Products.Queries;
using ShopApp.Grpc;

namespace ShopApp.Services;

public class ProductGrpcService : Grpc.ProductGrpcService.ProductGrpcServiceBase
{
    private readonly IMediator _mediator;

    public ProductGrpcService(IMediator mediator) => _mediator = mediator;

    public override async Task<ProductResponse> GetProduct(
        GetProductRequest request, 
        ServerCallContext context)
    {
        var product = await _mediator.Send(
            new GetProductByIdQuery(Guid.Parse(request.Id)));
        
        return new ProductResponse
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            Price = (double)product.Price,
            Stock = product.Stock
        };
    }

    public override async Task<ProductListResponse> GetProducts(
        EmptyRequest request, 
        ServerCallContext context)
    {
        var products = await _mediator.Send(new GetProductsQuery());
        
        var response = new ProductListResponse();
        response.Products.AddRange(products.Select(p => new ProductResponse
        {
            Id = p.Id.ToString(),
            Name = p.Name,
            Description = p.Description,
            Price = (double)p.Price,
            Stock = p.Stock
        }));
        
        return response;
    }
}
