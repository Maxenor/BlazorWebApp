using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<Location>> GetAllLocationsAsync();
    Task<Location?> GetLocationByIdAsync(int id);
}