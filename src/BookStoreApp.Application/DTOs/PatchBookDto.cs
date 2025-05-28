using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Application.DTOs;

/// <summary>
/// DTO for partial book updates using PATCH operations
/// All properties are nullable to support partial updates
/// </summary>
public class PatchBookDto
{
    /// <summary>
    /// Book title - optional for partial updates
    /// </summary>
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    /// <summary>
    /// Book author - optional for partial updates
    /// </summary>
    [StringLength(150, ErrorMessage = "Author cannot exceed 150 characters")]
    public string? Author { get; set; }

    /// <summary>
    /// Book ISBN - optional for partial updates
    /// </summary>
    [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
    public string? ISBN { get; set; }

    /// <summary>
    /// Book price - optional for partial updates
    /// </summary>
    [Range(0.01, 9999.99, ErrorMessage = "Price must be between 0.01 and 9999.99")]
    public decimal? Price { get; set; }

    /// <summary>
    /// Book description - optional for partial updates
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Number of pages - optional for partial updates
    /// </summary>
    [Range(1, 10000, ErrorMessage = "Page count must be between 1 and 10000")]
    public int? PageCount { get; set; }

    /// <summary>
    /// Publisher name - optional for partial updates
    /// </summary>
    [StringLength(100, ErrorMessage = "Publisher cannot exceed 100 characters")]
    public string? Publisher { get; set; }

    /// <summary>
    /// Publication date - optional for partial updates
    /// </summary>
    public DateTime? PublicationDate { get; set; }
}
