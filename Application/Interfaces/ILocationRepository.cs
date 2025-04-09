using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Interfaces;

public interface ILocationRepository
{
    Task<IEnumerable<Location>> GetAllAsync();
    Task<Location?> GetByIdAsync(int id);
}