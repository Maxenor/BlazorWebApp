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
            
            // Try to deserialize as array first
            try 
            {
                var events = JsonSerializer.Deserialize<IEnumerable<Event>>(content, _jsonOptions);
                return events ?? Enumerable.Empty<Event>();
            }
            catch (JsonException)
            {
                // If direct deserialization fails, try to deserialize as a wrapper object
                try
                {
                    // Try to deserialize as a wrapper object with various common property names
                    var wrapper = JsonSerializer.Deserialize<EventsWrapper>(content, _jsonOptions);
                    return wrapper?.Items ?? wrapper?.Events ?? wrapper?.Data ?? 
                           wrapper?.Results ?? Enumerable.Empty<Event>();
                }
                catch
                {
                    // If that also fails, try to deserialize a single event and wrap in a list
                    try
                    {
                        var singleEvent = JsonSerializer.Deserialize<Event>(content, _jsonOptions);
                        return singleEvent != null ? new[] { singleEvent } : Enumerable.Empty<Event>();
                    }
                    catch
                    {
                        Console.WriteLine("All deserialization attempts failed.");
                        return Enumerable.Empty<Event>();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching events: {ex.Message}");
            return Enumerable.Empty<Event>();
        }
    }
    
    // Class to handle API responses that wrap events in an object
    private class EventsWrapper
    {
        public IEnumerable<Event>? Items { get; set; }
        // Add other potential property names the API might use
        public IEnumerable<Event>? Events { get; set; }
        public IEnumerable<Event>? Data { get; set; }
        public IEnumerable<Event>? Results { get; set; }
    }

    public async Task<Event?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Event>($"{BaseUrl}/{id}");
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