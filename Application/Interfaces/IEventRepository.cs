using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Interfaces;

public interface IEventRepository
{
    Task<IEnumerable<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int id);
    Task<Event> CreateAsync(Event @event);
    Task<bool> DeleteAsync(int id);
    Task<Event> UpdateAsync(Event @event);
}