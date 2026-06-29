using FluentValidation;

namespace ProductService.Domain.Validators.Product;

internal static class ProductValidationRules
{
    internal static IRuleBuilderOptions<T, string?> IsProductName<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    ) where T : class
    {
        return ruleBuilder.NotEmpty().WithMessage("Product name cannot be empty!")
            .MinimumLength(8).WithMessage("Product name must be atleast 8 characters long!")
            .MaximumLength(60).WithMessage("Product name length must not exceed 60 characters!");
    }

    internal static IRuleBuilderOptions<T, string?> IsProductDescription<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Product description cannot be empty!")
            .MaximumLength(200).WithMessage("Product description length must not exceed 200 characters!");
    }

    internal static IRuleBuilderOptions<T, decimal> IsProductPrice<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0.49m)
            .WithMessage("Product price must be greater than 49 cents!");
    }

    internal static IRuleBuilderOptions<T, decimal?> IsProductPrice<T>(this IRuleBuilder<T, decimal?> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(0.49m)
            .WithMessage("Product price must be greater than 49 cents!");
    }
}