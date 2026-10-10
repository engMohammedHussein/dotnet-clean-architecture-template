namespace CleanArchitecture.Template.Domain.Exceptions;

/// <summary>
/// Thrown when a product is created or renamed to a SKU that already exists.
/// SKU uniqueness is a business rule, so it lives in the domain, not as a
/// database constraint error leaking upward.
/// </summary>
public class DuplicateSkuException : DomainException
{
    public DuplicateSkuException(string sku)
        : base($"A product with SKU \"{sku}\" already exists.")
    {
    }
}
