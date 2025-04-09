using System.ComponentModel.DataAnnotations;

namespace BlazorWebApp.Application.DTOs;

public class LocationDto
{
    [Required(ErrorMessage = "Location name is required")]
    [StringLength(100, ErrorMessage = "Location name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters")]
    public string Address { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "City cannot be longer than 100 characters")]
    public string? City { get; set; }

    [StringLength(100, ErrorMessage = "State cannot be longer than 100 characters")]
    public string? State { get; set; }

    [StringLength(20, ErrorMessage = "Postal code cannot be longer than 20 characters")]
    public string? PostalCode { get; set; }

    [StringLength(100, ErrorMessage = "Country cannot be longer than 100 characters")]
    public string? Country { get; set; }
}