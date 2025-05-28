using BookStoreApp.Domain.Entities;

namespace BookStoreApp.Application.Interfaces;

/// <summary>
/// Repository interface for book data access operations
/// This interface abstracts the data access layer and follows the Repository pattern
/// It defines the contract for data operations without specifying the implementation
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Retrieves all books from the data store
    /// </summary>
    /// <returns>Collection of all Book entities</returns>
    Task<IEnumerable<Book>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific book by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the book</param>
    /// <returns>Book entity if found, null if not found</returns>
    Task<Book?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a book by its ISBN
    /// Used for uniqueness validation and ISBN-based lookups
    /// </summary>
    /// <param name="isbn">The ISBN of the book</param>
    /// <returns>Book entity if found, null if not found</returns>
    Task<Book?> GetByIsbnAsync(string isbn);

    /// <summary>
    /// Adds a new book to the data store
    /// </summary>
    /// <param name="book">The book entity to add</param>
    /// <returns>The added book entity with any generated values</returns>
    Task<Book> AddAsync(Book book);

    /// <summary>
    /// Updates an existing book in the data store
    /// </summary>
    /// <param name="book">The book entity to update</param>
    /// <returns>The updated book entity</returns>
    Task<Book> UpdateAsync(Book book);

    /// <summary>
    /// Removes a book from the data store
    /// </summary>
    /// <param name="book">The book entity to remove</param>
    /// <returns>Task representing the async operation</returns>
    Task DeleteAsync(Book book);

    /// <summary>
    /// Searches for books by title or author using case-insensitive partial matching
    /// </summary>
    /// <param name="searchTerm">The search term to match against title or author</param>
    /// <returns>Collection of matching Book entities</returns>
    Task<IEnumerable<Book>> SearchAsync(string searchTerm);

    /// <summary>
    /// Retrieves books published within a specific date range
    /// </summary>
    /// <param name="startDate">Start date of the publication range</param>
    /// <param name="endDate">End date of the publication range</param>
    /// <returns>Collection of Book entities published within the range</returns>
    Task<IEnumerable<Book>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Checks if a book with the given ISBN already exists
    /// Used for validation before creating or updating books
    /// </summary>
    /// <param name="isbn">The ISBN to check</param>
    /// <param name="excludeId">Optional ID to exclude from the check (for updates)</param>
    /// <returns>True if ISBN exists, false otherwise</returns>
    Task<bool> IsbnExistsAsync(string isbn, Guid? excludeId = null);
}
