using BookStoreApp.Domain.Common;

namespace BookStoreApp.Domain.Entities;

/// <summary>
/// Book entity representing a book in our bookstore
/// This is the core domain model that contains business rules and properties
/// </summary>
public class Book : BaseEntity
{
    /// <summary>
    /// The title of the book
    /// Required field with maximum length validation
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The author(s) of the book
    /// Required field that can contain multiple authors separated by commas
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// International Standard Book Number (ISBN)
    /// Must be unique across all books in the system
    /// Typically follows ISBN-13 format (13 digits)
    /// </summary>
    public string ISBN { get; set; } = string.Empty;

    /// <summary>
    /// The price of the book in decimal format
    /// Supports currency with two decimal places
    /// Must be greater than 0 (business rule)
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The date when the book was published
    /// Cannot be in the future (business rule)
    /// </summary>
    public DateTime PublicationDate { get; set; }

    /// <summary>
    /// Brief description or summary of the book
    /// Optional field for additional book information
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Number of pages in the book
    /// Optional field, must be positive if provided
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// The publisher of the book
    /// Optional field for publisher information
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// Default constructor for Entity Framework
    /// EF Core requires a parameterless constructor
    /// </summary>
    public Book() { }

    /// <summary>
    /// Constructor with required parameters
    /// This ensures that a book cannot be created without essential information
    /// </summary>
    /// <param name="title">Book title (required)</param>
    /// <param name="author">Book author (required)</param>
    /// <param name="isbn">Book ISBN (required and must be unique)</param>
    /// <param name="price">Book price (required and must be > 0)</param>
    /// <param name="publicationDate">Publication date (required, cannot be future)</param>
    public Book(string title, string author, string isbn, decimal price, DateTime publicationDate)
    {
        // Validate required parameters
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));
        
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty", nameof(isbn));
        
        if (price <= 0)
            throw new ArgumentException("Price must be greater than 0", nameof(price));
        
        if (publicationDate > DateTime.Now)
            throw new ArgumentException("Publication date cannot be in the future", nameof(publicationDate));

        // Set the properties
        Title = title;
        Author = author;
        ISBN = isbn;
        Price = price;
        PublicationDate = publicationDate;
    }

    /// <summary>
    /// Updates the book information
    /// This method encapsulates the business logic for updating a book
    /// </summary>
    /// <param name="title">New title</param>
    /// <param name="author">New author</param>
    /// <param name="price">New price</param>
    /// <param name="description">New description</param>
    public void UpdateBookInfo(string title, string author, decimal price, string? description = null)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty", nameof(author));
        
        if (price <= 0)
            throw new ArgumentException("Price must be greater than 0", nameof(price));

        // Update properties
        Title = title;
        Author = author;
        Price = price;
        Description = description;
        
        // Mark the entity as updated (from BaseEntity)
        MarkAsUpdated();
    }

    /// <summary>
    /// Checks if the book is a recent publication (published within last 2 years)
    /// This is a business rule that might be used for promotions or categorization
    /// </summary>
    /// <returns>True if published within last 2 years, false otherwise</returns>
    public bool IsRecentPublication()
    {
        return PublicationDate >= DateTime.Now.AddYears(-2);
    }

    /// <summary>
    /// Gets a formatted display name for the book
    /// Useful for UI display purposes
    /// </summary>
    /// <returns>Formatted string with title and author</returns>
    public string GetDisplayName()
    {
        return $"{Title} by {Author}";
    }
}
