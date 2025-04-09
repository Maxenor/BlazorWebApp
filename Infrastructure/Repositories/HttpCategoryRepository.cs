using System.Net.Http.Json;
using BlazorWebApp.Application.Interfaces;
using BlazorWebApp.Domain.Entities;

namespace BlazorWebApp.Infrastructure.Repositories;

public class HttpCategoryRepository : ICategoryRepository
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "/api/v1/Categories";

    public HttpCategoryRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<IEnumerable<Category>>(BaseUrl) ?? Enumerable.Empty<Category>();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Category>($"{BaseUrl}/{id}");
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, category);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Category>() ?? throw new InvalidOperationException("Failed to create category");
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{category.Id}", category);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Category>() ?? throw new InvalidOperationException("Failed to update category");
    }
}