using BlazorWebApp.Application.DTOs;
using BlazorWebApp.Application.Interfaces;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description
        };

        return await _categoryRepository.CreateAsync(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<Category> UpdateCategoryAsync(int id, CategoryDto categoryDto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Category with ID {id} not found");

        existingCategory.Name = categoryDto.Name;
        existingCategory.Description = categoryDto.Description;

        return await _categoryRepository.UpdateAsync(existingCategory);
    }
}