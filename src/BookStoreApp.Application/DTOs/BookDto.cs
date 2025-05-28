namespace BookStoreApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for returning book information
/// This DTO includes all book information that should be exposed to clients
/// It includes read-only fields like Id, CreatedAt, UpdatedAt for client reference
/// </summary>
public class BookDto
{
    /// <summary>
    /// Unique identifier of the book
    /// Auto-generated and read-only for clients
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The title of the book
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The author(s) of the book
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// International Standard Book Number
    /// Unique identifier for the book in the publishing industry
    /// </summary>
    public string ISBN { get; set; } = string.Empty;

    /// <summary>
    /// The current price of the book
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The date when the book was published
    /// </summary>
    public DateTime PublicationDate { get; set; }

    /// <summary>
    /// Description or summary of the book
    /// May be null if not provided
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Number of pages in the book
    /// May be null if not specified
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// Publisher of the book
    /// May be null if not specified
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// When the book record was created in the system
    /// Useful for auditing and client-side caching
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the book record was last updated
    /// Useful for conflict resolution and client-side caching
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Computed property indicating if this is a recent publication
    /// Business logic exposed as a convenience for clients
    /// </summary>
    public bool IsRecentPublication { get; set; }

    /// <summary>
    /// Formatted display name for UI purposes
    /// Computed property that combines title and author
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}
