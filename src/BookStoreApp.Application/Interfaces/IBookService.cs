using BookStoreApp.Application.DTOs;

namespace BookStoreApp.Application.Interfaces;

/// <summary>
/// Service interface for book-related operations
/// This interface defines the contract for book business logic
/// Following the Interface Segregation Principle - focused only on book operations
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Retrieves all books from the system
    /// Returns a collection of BookDto objects for client consumption
    /// </summary>
    /// <returns>Collection of all books as DTOs</returns>
    Task<IEnumerable<BookDto>> GetAllBooksAsync();

    /// <summary>
    /// Retrieves a specific book by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the book</param>
    /// <returns>BookDto if found, null if not found</returns>
    Task<BookDto?> GetBookByIdAsync(Guid id);

    /// <summary>
    /// Retrieves a book by its ISBN
    /// Useful for checking uniqueness and lookups by ISBN
    /// </summary>
    /// <param name="isbn">The ISBN of the book to find</param>
    /// <returns>BookDto if found, null if not found</returns>
    Task<BookDto?> GetBookByIsbnAsync(string isbn);

    /// <summary>
    /// Creates a new book in the system
    /// Validates business rules before creation
    /// </summary>
    /// <param name="createBookDto">The book information to create</param>
    /// <returns>The created book as a BookDto</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    /// <exception cref="InvalidOperationException">Thrown when ISBN already exists</exception>
    Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);

    /// <summary>
    /// Updates an existing book's information
    /// Validates business rules before updating
    /// </summary>
    /// <param name="id">The unique identifier of the book to update</param>
    /// <param name="updateBookDto">The updated book information</param>
    /// <returns>The updated book as a BookDto</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    /// <exception cref="KeyNotFoundException">Thrown when book with given ID is not found</exception>
    Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto);

    /// <summary>
    /// Deletes a book from the system
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete</param>
    /// <returns>True if deleted successfully, false if book not found</returns>
    Task<bool> DeleteBookAsync(Guid id);

    /// <summary>
    /// Searches books by title or author
    /// Performs case-insensitive partial matching
    /// </summary>
    /// <param name="searchTerm">The term to search for in title or author</param>
    /// <returns>Collection of matching books as DTOs</returns>
    Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);

    /// <summary>
    /// Gets books published within a specific date range
    /// Useful for filtering by publication period
    /// </summary>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <returns>Collection of books published within the date range</returns>
    Task<IEnumerable<BookDto>> GetBooksByDateRangeAsync(DateTime startDate, DateTime endDate);    /// <summary>
    /// Partially updates a book with only the provided fields
    /// Only non-null properties in the patch DTO will be updated
    /// </summary>
    /// <param name="id">The unique identifier of the book to update</param>
    /// <param name="patchBookDto">DTO containing the fields to update</param>
    /// <returns>Updated BookDto if successful, null if book not found</returns>
    Task<BookDto?> PatchBookAsync(Guid id, PatchBookDto patchBookDto);
}
