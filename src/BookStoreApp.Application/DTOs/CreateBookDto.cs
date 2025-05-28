namespace BookStoreApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for creating a new book
/// This DTO contains only the fields needed when creating a book
/// It excludes auto-generated fields like Id, CreatedAt, UpdatedAt
/// </summary>
public class CreateBookDto
{
    /// <summary>
    /// The title of the book
    /// Required field - cannot be null or empty
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The author(s) of the book
    /// Required field - cannot be null or empty
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// International Standard Book Number
    /// Must be unique in the system
    /// Required field - cannot be null or empty
    /// </summary>
    public string ISBN { get; set; } = string.Empty;

    /// <summary>
    /// The price of the book
    /// Must be greater than 0
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The date when the book was published
    /// Cannot be in the future
    /// </summary>
    public DateTime PublicationDate { get; set; }

    /// <summary>
    /// Optional description of the book
    /// Can be null or empty
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional number of pages
    /// If provided, must be positive
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Optional publisher information
    /// Can be null or empty
    /// </summary>
    public string? Publisher { get; set; }
}
