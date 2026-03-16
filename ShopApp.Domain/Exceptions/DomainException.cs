namespace ShopApp.Domain.Exceptions;

public class DomainException : Exception
{
  public DomainException(string message) : base(message) { }
}

public class NotFoundException : DomainException
{
  public NotFoundException(string entity, int id)
    : base($"{entity} with id {id} not found") { }
  
  public NotFoundException(string entity, Guid id)
    : base($"{entity} with id {id} not found") { }
}