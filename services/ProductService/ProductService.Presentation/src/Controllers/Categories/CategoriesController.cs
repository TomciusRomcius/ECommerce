using ECommerceBackend.Utils.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Domain.Utils;
using ProductService.Presentation.Controllers.Categories.Dtos;
using ProductService.Presentation.Utils;

namespace ProductService.Presentation.Controllers.Categories;

[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(
        string searchText = "",
        CancellationToken cancellationToken = default)
    {
        List<CategoryEntity> categories = await _categoriesService.GetCategoriesAsync(searchText, cancellationToken);
        return Ok(categories);
    }

    [Authorize(Roles = RoleTypes.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] RequestCreateCategoryDto createCategoryDto)
    {
        Result<int> result = await _categoriesService.CreateCategoryAsync(new CategoryEntity(createCategoryDto.Name));
        if (result.Errors.Any()) return ControllerUtils.ResultErrorToResponse(result.Errors.First());
        int categoryId = result.GetValue();
        
        return Created(nameof(CreateCategory), new
        {
            categoryId
        });
    }
}