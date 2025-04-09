using BlazorWebApp.Application.DTOs;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Interfaces;

public interface IEventService
{
    Task<IEnumerable<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task<Event> CreateEventAsync(EventDto eventDto);
    Task<bool> DeleteEventAsync(int id);
    Task<Event> UpdateEventAsync(int id, EventDto eventDto);
}