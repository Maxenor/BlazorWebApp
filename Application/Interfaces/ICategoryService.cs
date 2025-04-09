using BlazorWebApp.Application.DTOs;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(CategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(int id);
    Task<Category> UpdateCategoryAsync(int id, CategoryDto categoryDto);
}