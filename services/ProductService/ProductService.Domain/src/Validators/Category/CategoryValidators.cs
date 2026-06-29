using FluentValidation;
using ProductService.Domain.Entities;
using ProductService.Domain.Models;

namespace ProductService.Domain.Validators.Category;

public class CategoryEntityValidators : AbstractValidator<CategoryEntity>
{
    public CategoryEntityValidators()
    {
        RuleFor(x => x.Name).IsCategoryName();
    }
}

public class UpdateCategoryModelValidator : AbstractValidator<UpdateCategoryModel>
{
    public UpdateCategoryModelValidator()
    {
        RuleFor(x => x.Name).IsCategoryName().When(x => x.Name != null);
    }
}