using BlazorWebApp.Application.DTOs;
using BlazorWebApp.Application.Interfaces;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;

    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<IEnumerable<Event>> GetAllEventsAsync()
    {
        return await _eventRepository.GetAllAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _eventRepository.GetByIdAsync(id);
    }

    public async Task<Event> CreateEventAsync(EventDto eventDto)
    {
        var @event = new Event
        {
            Title = eventDto.Title,
            Description = eventDto.Description,
            StartDate = eventDto.StartDate,
            EndDate = eventDto.EndDate,
            CategoryId = eventDto.CategoryId,
            LocationId = eventDto.LocationId
        };

        return await _eventRepository.CreateAsync(@event);
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        return await _eventRepository.DeleteAsync(id);
    }

    public async Task<Event> UpdateEventAsync(int id, EventDto eventDto)
    {
        var existingEvent = await _eventRepository.GetByIdAsync(id) 
            ?? throw new InvalidOperationException($"Event with ID {id} not found");

        existingEvent.Title = eventDto.Title;
        existingEvent.Description = eventDto.Description;
        existingEvent.StartDate = eventDto.StartDate;
        existingEvent.EndDate = eventDto.EndDate;
        existingEvent.CategoryId = eventDto.CategoryId;
        existingEvent.LocationId = eventDto.LocationId;

        return await _eventRepository.UpdateAsync(existingEvent);
    }
}