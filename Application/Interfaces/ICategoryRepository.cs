using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(Category category);
    Task<bool> DeleteAsync(int id);
    Task<Category> UpdateAsync(Category category);
}