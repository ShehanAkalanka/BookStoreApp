namespace BookStoreApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for updating an existing book
/// Similar to CreateBookDto but used for update operations
/// Excludes Id since it's provided in the route, and excludes auto-generated audit fields
/// </summary>
public class UpdateBookDto
{
    /// <summary>
    /// The updated title of the book
    /// Required field - cannot be null or empty
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The updated author(s) of the book
    /// Required field - cannot be null or empty
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// The updated price of the book
    /// Must be greater than 0
    /// Note: ISBN is typically not updated after creation, so it's excluded
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Updated description of the book
    /// Optional field - can be null or empty
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Updated number of pages
    /// Optional field - if provided, must be positive
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Updated publisher information
    /// Optional field - can be null or empty
    /// </summary>
    public string? Publisher { get; set; }
}
