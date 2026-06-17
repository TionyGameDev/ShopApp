using MediatR;

namespace ShopApp.Application.CQRS.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest;