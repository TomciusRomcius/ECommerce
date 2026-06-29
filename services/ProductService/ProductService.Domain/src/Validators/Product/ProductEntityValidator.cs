using FluentValidation;
using ProductService.Domain.Entities;

namespace ProductService.Domain.Validators.Product;

public class ProductEntityValidator : AbstractValidator<ProductEntity>
{
    public ProductEntityValidator()
    {
        RuleFor(x => x.Name).IsProductName();
        RuleFor(x => x.Description).IsProductDescription();
        RuleFor(x => x.Price).IsProductPrice();
    }
}