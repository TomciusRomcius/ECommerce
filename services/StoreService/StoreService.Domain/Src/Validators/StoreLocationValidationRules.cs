using FluentValidation;

namespace StoreService.Domain.Validators;

internal static class StoreLocationValidationRules
{
    internal static IRuleBuilderOptions<T, string?> IsStoreLocationDisplayName<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    ) where T : class
    {
        return ruleBuilder.NotEmpty().WithMessage("StoreLocation display name cannot be empty!")
            .MinimumLength(8).WithMessage("StoreLocation display name must be atleast 4 characters long!")
            .MaximumLength(60).WithMessage("StoreLocation display name length must not exceed 60 characters!");
    }

    internal static IRuleBuilderOptions<T, string?> IsStoreLocationAddress<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    ) where T : class
    {
        return ruleBuilder.NotEmpty().WithMessage("StoreLocation display name cannot be empty!")
            .MinimumLength(8).WithMessage("StoreLocation display name must be atleast 4 characters long!")
            .MaximumLength(60).WithMessage("StoreLocation display name length must not exceed 60 characters!");
    }
}