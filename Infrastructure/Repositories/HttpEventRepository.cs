using System.Net.Http.Json;
using System.Text.Json;
using BlazorWebApp.Application.Interfaces;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Infrastructure.Repositories;

public class HttpEventRepository : IEventRepository
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/v1/events";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpEventRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {content}"); // Log the raw response for debugging
            
            if (string.IsNullOrEmpty(content))
                return Enumerable.Empty<Event>();
            
            // Try to deserialize using the pagination wrapper first (items property)
            try 
            {
                var wrapper = JsonSerializer.Deserialize<PaginatedResponse>(content, _jsonOptions);
                if (wrapper?.Items != null)
                {
                    Console.WriteLine($"Successfully parsed {wrapper.Items.Count()} events with pagination");
                    foreach (var evt in wrapper.Items)
                    {
                        Console.WriteLine($"Event: {evt.Id} - {evt.Title}, Category: {evt.Category?.Name}, Location: {evt.Location?.Name}");
                    }
                    
                    // Update CategoryId and LocationId from nested objects if needed
                    foreach (var evt in wrapper.Items)
                    {
                        if (evt.Category != null && evt.CategoryId == 0)
                            evt.CategoryId = evt.Category.Id;
                            
                        if (evt.Location != null && evt.LocationId == 0)
                            evt.LocationId = evt.Location.Id;
                    }
                    
                    return wrapper.Items;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse paginated response: {ex.Message}");
                // Continue to try other formats
            }
            
            // Try to deserialize as array directly
            try 
            {
                var events = JsonSerializer.Deserialize<IEnumerable<Event>>(content, _jsonOptions);
                if (events != null)
                {
                    Console.WriteLine($"Successfully parsed {events.Count()} events as direct array");
                    return events;
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse as direct array: {ex.Message}");
            }
            
            // Try to deserialize as a single event
            try
            {
                var singleEvent = JsonSerializer.Deserialize<Event>(content, _jsonOptions);
                if (singleEvent != null)
                {
                    Console.WriteLine($"Successfully parsed single event: {singleEvent.Id} - {singleEvent.Title}");
                    return new[] { singleEvent };
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to parse as single event: {ex.Message}");
            }
            
            Console.WriteLine("All parsing attempts failed, returning empty collection");
            return Enumerable.Empty<Event>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching events: {ex.Message}");
            return Enumerable.Empty<Event>();
        }
    }
    
    // Pagination response wrapper
    private class PaginatedResponse
    {
        public IEnumerable<Event> Items { get; set; } = Enumerable.Empty<Event>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response for event {id}: {content}");
            
            if (string.IsNullOrEmpty(content))
                return null;
                
            var @event = JsonSerializer.Deserialize<Event>(content, _jsonOptions);
            
            // Update IDs from nested objects if needed
            if (@event?.Category != null && @event.CategoryId == 0)
                @event.CategoryId = @event.Category.Id;
                
            if (@event?.Location != null && @event.LocationId == 0)
                @event.LocationId = @event.Location.Id;
                
            return @event;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching event {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Event> CreateAsync(Event @event)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, @event);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Event>() ?? throw new InvalidOperationException("Failed to create event");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Event> UpdateAsync(Event @event)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{@event.Id}", @event);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Event>() ?? throw new InvalidOperationException("Failed to update event");
    }
}