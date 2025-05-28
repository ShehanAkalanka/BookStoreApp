using BookStoreApp.Application.Interfaces;
using BookStoreApp.Domain.Entities;
using BookStoreApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Book entity data access
/// This class implements the IBookRepository interface using Entity Framework Core
/// It handles all database operations for Book entities
/// </summary>
public class BookRepository : IBookRepository
{
    // Database context for data access
    private readonly BookStoreDbContext _context;

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    /// <param name="context">Database context for book operations</param>
    public BookRepository(BookStoreDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves all books from the database
    /// Orders by title for consistent results
    /// </summary>
    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .OrderBy(b => b.Title) // Consistent ordering
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific book by its unique identifier
    /// </summary>
    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    /// Retrieves a book by its ISBN
    /// Uses case-insensitive comparison for better user experience
    /// </summary>
    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        return await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN.ToLower() == isbn.ToLower());
    }

    /// <summary>
    /// Adds a new book to the database
    /// </summary>
    public async Task<Book> AddAsync(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        // Add the book to the context
        _context.Books.Add(book);
        
        // Save changes to the database
        await _context.SaveChangesAsync();
        
        // Return the book with any generated values (like timestamps)
        return book;
    }

    /// <summary>
    /// Updates an existing book in the database
    /// </summary>
    public async Task<Book> UpdateAsync(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        // Mark the entity as modified
        _context.Books.Update(book);
        
        // Save changes to the database
        await _context.SaveChangesAsync();
        
        // Return the updated book
        return book;
    }

    /// <summary>
    /// Removes a book from the database
    /// </summary>
    public async Task DeleteAsync(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));

        // Remove the book from the context
        _context.Books.Remove(book);
        
        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Searches for books by title or author using case-insensitive partial matching
    /// Uses LIKE operator for flexible searching
    /// </summary>
    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Book>();

        // Convert search term to lowercase for case-insensitive search
        var lowerSearchTerm = searchTerm.ToLower();

        return await _context.Books
            .Where(b => b.Title.ToLower().Contains(lowerSearchTerm) || 
                       b.Author.ToLower().Contains(lowerSearchTerm))
            .OrderBy(b => b.Title) // Consistent ordering
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves books published within a specific date range
    /// Includes both start and end dates in the range
    /// </summary>
    public async Task<IEnumerable<Book>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Books
            .Where(b => b.PublicationDate >= startDate && b.PublicationDate <= endDate)
            .OrderBy(b => b.PublicationDate) // Order by publication date
            .ThenBy(b => b.Title) // Then by title for books published on same date
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a book with the given ISBN already exists
    /// Supports excluding a specific book ID (useful for updates)
    /// </summary>
    public async Task<bool> IsbnExistsAsync(string isbn, Guid? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            return false;

        var query = _context.Books.Where(b => b.ISBN.ToLower() == isbn.ToLower());
        
        // If excludeId is provided, exclude that book from the check
        if (excludeId.HasValue)
        {
            query = query.Where(b => b.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
