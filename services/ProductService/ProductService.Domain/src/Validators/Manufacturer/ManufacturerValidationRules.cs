using FluentValidation;

namespace ProductService.Domain.Validators.Manufacturer;

internal static class ManufacturerValidationRules
{
    internal static IRuleBuilderOptions<T, string?> IsManufacturerName<T>(
        this IRuleBuilder<T, string?> ruleBuilder) where T : class
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Manufacturer name cannot be empty!")
            .MinimumLength(3).WithMessage("Manufacturer name be atleast 3 characters long!")
            .MaximumLength(20).WithMessage("Manufacturer name length must not exceed 20 characters");
    }
}