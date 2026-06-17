using System.Transactions;
using MediatR;
using ShopApp.Application.CQRS.Products.Commands;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Mappers;
using ShopApp.Application.Transations;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.CQRS.Products.Handlers;

public class UpdateProductHandler(IProductRepository repository, ICacheService cache,IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.RepeatableRead);
    
        var productToUpdate = await repository.GetByIdAsync(command.Dto.Id);
        if (productToUpdate != null)
        {
            productToUpdate.Name = command.Dto.Name;
            productToUpdate.Price = command.Dto.Price;
            productToUpdate.Stock = command.Dto.Stock;
            productToUpdate.Description = command.Dto.Description;
            await repository.UpdateAsync(productToUpdate);
            
            await cache.RemoveAsync("products"); 
            await cache.SetAsync(productToUpdate.Id.ToString(),productToUpdate);
      
            await unitOfWork.CommitAsync();
            return productToUpdate.ToDto();
        }
    
        await unitOfWork.RollbackAsync();
        throw new NotFoundException("Product", command.Dto.Id);
    }
}