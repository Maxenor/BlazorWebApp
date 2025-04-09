using System.Text.Json;
using BlazorWebApp.Application.Interfaces;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Infrastructure.Repositories;

public class HttpLocationRepository : ILocationRepository
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/v1/Locations";
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HttpLocationRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Location>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Location API Response: {content}"); // Log raw response
                        
            if (string.IsNullOrEmpty(content))
                return Enumerable.Empty<Location>();
                
            try 
            {
                var locations = JsonSerializer.Deserialize<IEnumerable<Location>>(content, _jsonOptions);
                if (locations != null)
                {
                    foreach (var loc in locations)
                    {
                        Console.WriteLine($"Parsed Location: ID={loc.Id}, Name={loc.Name}");
                    }
                }
                return locations ?? Enumerable.Empty<Location>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Primary location deserialization failed: {ex.Message}");
                // Try alternative deserialization approaches like with Events
                try
                {
                    var wrapper = JsonSerializer.Deserialize<LocationWrapper>(content, _jsonOptions);
                    var result = wrapper?.Items ?? wrapper?.Locations ?? wrapper?.Data ?? 
                           wrapper?.Results ?? Enumerable.Empty<Location>();
                    
                    // Log the results
                    foreach (var loc in result)
                    {
                        Console.WriteLine($"Wrapper Location: ID={loc.Id}, Name={loc.Name}");
                    }
                    
                    return result;
                }
                catch (Exception innerEx)
                {
                    Console.WriteLine($"Wrapper deserialization failed: {innerEx.Message}");
                    try
                    {
                        var singleLocation = JsonSerializer.Deserialize<Location>(content, _jsonOptions);
                        if (singleLocation != null)
                        {
                            Console.WriteLine($"Single Location: ID={singleLocation.Id}, Name={singleLocation.Name}");
                            return new[] { singleLocation };
                        }
                        return Enumerable.Empty<Location>();
                    }
                    catch (Exception finalEx)
                    {
                        Console.WriteLine($"All location deserialization attempts failed: {finalEx.Message}");
                        return Enumerable.Empty<Location>();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching locations: {ex.Message}");
            return Enumerable.Empty<Location>();
        }
    }

    public async Task<Location?> GetByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
                return null;
                
            return JsonSerializer.Deserialize<Location>(content, _jsonOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching location {id}: {ex.Message}");
            return null;
        }
    }
    
    private class LocationWrapper
    {
        public IEnumerable<Location>? Items { get; set; }
        public IEnumerable<Location>? Locations { get; set; }
        public IEnumerable<Location>? Data { get; set; }
        public IEnumerable<Location>? Results { get; set; }
    }
}