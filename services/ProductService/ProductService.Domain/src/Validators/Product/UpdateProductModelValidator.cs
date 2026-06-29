using FluentValidation;
using ProductService.Domain.Models;

namespace ProductService.Domain.Validators.Product;

public class UpdateProductModelValidator : AbstractValidator<UpdateProductModel>
{
    public UpdateProductModelValidator()
    {
        RuleFor(x => x.Name).IsProductName().When(x => x.Name != null);
        RuleFor(x => x.Description).IsProductDescription().When(x => x.Description != null);
        RuleFor(x => x.Price!.Value).IsProductPrice().When(x => x.Price != null);
    }
}