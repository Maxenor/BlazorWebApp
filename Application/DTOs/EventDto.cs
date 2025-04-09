using System.ComponentModel.DataAnnotations;

namespace BlazorWebApp.Application.DTOs;

public class EventDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description cannot be longer than 2000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Location is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid location")]
    public int LocationId { get; set; }
}