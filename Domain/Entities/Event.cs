namespace BlazorWebApp.Domain.Entities;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Status { get; set; }
    
    // IDs for reference
    public int CategoryId { get; set; }
    public int LocationId { get; set; }
    
    // Navigation properties for nested objects from API
    public Category? Category { get; set; }
    public Location? Location { get; set; }
}