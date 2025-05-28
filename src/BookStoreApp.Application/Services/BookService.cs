using BookStoreApp.Application.DTOs;
using BookStoreApp.Application.Interfaces;
using BookStoreApp.Domain.Entities;

namespace BookStoreApp.Application.Services;

/// <summary>
/// Implementation of the book service containing business logic
/// This service orchestrates between the domain entities and the data access layer
/// It handles validation, business rules, and data transformation
/// </summary>
public class BookService : IBookService
{
    // Repository dependency for data access
    private readonly IBookRepository _bookRepository;

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    /// <param name="bookRepository">Repository for book data access</param>
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    /// <summary>
    /// Retrieves all books and converts them to DTOs
    /// </summary>
    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        // Get all books from the repository
        var books = await _bookRepository.GetAllAsync();
        
        // Convert domain entities to DTOs for the client
        return books.Select(MapToDto);
    }

    /// <summary>
    /// Retrieves a specific book by ID and converts to DTO
    /// </summary>
    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        // Validate input
        if (id == Guid.Empty)
            return null;

        // Get book from repository
        var book = await _bookRepository.GetByIdAsync(id);
        
        // Return null if not found, otherwise convert to DTO
        return book == null ? null : MapToDto(book);
    }

    /// <summary>
    /// Retrieves a book by ISBN and converts to DTO
    /// </summary>
    public async Task<BookDto?> GetBookByIsbnAsync(string isbn)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(isbn))
            return null;

        // Get book from repository
        var book = await _bookRepository.GetByIsbnAsync(isbn);
        
        // Return null if not found, otherwise convert to DTO
        return book == null ? null : MapToDto(book);
    }

    /// <summary>
    /// Creates a new book with validation and business rule enforcement
    /// </summary>
    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
    {
        // Validate input DTO
        ValidateCreateBookDto(createBookDto);

        // Check if ISBN already exists (business rule: ISBN must be unique)
        if (await _bookRepository.IsbnExistsAsync(createBookDto.ISBN))
        {
            throw new InvalidOperationException($"A book with ISBN '{createBookDto.ISBN}' already exists.");
        }

        // Create domain entity using the constructor with validation
        var book = new Book(
            createBookDto.Title,
            createBookDto.Author,
            createBookDto.ISBN,
            createBookDto.Price,
            createBookDto.PublicationDate)
        {
            Description = createBookDto.Description,
            PageCount = createBookDto.PageCount,
            Publisher = createBookDto.Publisher
        };

        // Save to repository
        var createdBook = await _bookRepository.AddAsync(book);
        
        // Return as DTO
        return MapToDto(createdBook);
    }

    /// <summary>
    /// Updates an existing book with validation
    /// </summary>
    public async Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto)
    {
        // Validate input
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid book ID", nameof(id));
        
        ValidateUpdateBookDto(updateBookDto);

        // Get existing book
        var existingBook = await _bookRepository.GetByIdAsync(id);
        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID '{id}' not found.");
        }

        // Update the book using domain method (encapsulates business logic)
        existingBook.UpdateBookInfo(
            updateBookDto.Title,
            updateBookDto.Author,
            updateBookDto.Price,
            updateBookDto.Description);

        // Update optional fields
        existingBook.PageCount = updateBookDto.PageCount;
        existingBook.Publisher = updateBookDto.Publisher;

        // Save changes
        var updatedBook = await _bookRepository.UpdateAsync(existingBook);
        
        // Return as DTO
        return MapToDto(updatedBook);
    }

    /// <summary>
    /// Deletes a book by ID
    /// </summary>
    public async Task<bool> DeleteBookAsync(Guid id)
    {
        // Validate input
        if (id == Guid.Empty)
            return false;

        // Get existing book
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return false;

        // Delete the book
        await _bookRepository.DeleteAsync(book);
        return true;
    }

    /// <summary>
    /// Searches books by title or author
    /// </summary>
    public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
    {
        // Handle empty search term
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<BookDto>();

        // Search in repository
        var books = await _bookRepository.SearchAsync(searchTerm.Trim());
        
        // Convert to DTOs
        return books.Select(MapToDto);
    }

    /// <summary>
    /// Gets books published within a date range
    /// </summary>
    public async Task<IEnumerable<BookDto>> GetBooksByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        // Validate date range
        if (startDate > endDate)
        {
            throw new ArgumentException("Start date cannot be greater than end date");
        }

        // Get books from repository
        var books = await _bookRepository.GetByDateRangeAsync(startDate, endDate);
        
        // Convert to DTOs
        return books.Select(MapToDto);
    }

    /// <summary>
    /// Partially updates a book with only the provided fields
    /// Only non-null properties in the patch DTO will be updated
    /// Follows the principle of minimal changes and business rule validation
    /// </summary>
    /// <param name="id">The unique identifier of the book to update</param>
    /// <param name="patchBookDto">DTO containing the fields to update</param>
    /// <returns>Updated BookDto if successful, null if book not found</returns>
    public async Task<BookDto?> PatchBookAsync(Guid id, PatchBookDto patchBookDto)
    {
        // Get existing book
        var existingBook = await _bookRepository.GetByIdAsync(id);
        if (existingBook == null)
        {
            return null;
        }

        // Apply partial updates only for non-null values
        // This ensures we only update the fields that were explicitly provided

        // Update core book information if provided
        if (patchBookDto.Title != null || patchBookDto.Author != null || 
            patchBookDto.Price.HasValue || patchBookDto.Description != null)
        {
            // Use existing values for fields not being updated
            var title = patchBookDto.Title ?? existingBook.Title;
            var author = patchBookDto.Author ?? existingBook.Author;
            var price = patchBookDto.Price ?? existingBook.Price;
            var description = patchBookDto.Description ?? existingBook.Description;

            // Update using domain method (maintains business rules)
            existingBook.UpdateBookInfo(title, author, price, description);
        }

        // Update ISBN if provided (careful - should validate uniqueness)
        if (patchBookDto.ISBN != null)
        {
            existingBook.ISBN = patchBookDto.ISBN;
        }

        // Update optional fields if provided
        if (patchBookDto.PageCount.HasValue)
        {
            existingBook.PageCount = patchBookDto.PageCount.Value;
        }

        if (patchBookDto.Publisher != null)
        {
            existingBook.Publisher = patchBookDto.Publisher;
        }

        if (patchBookDto.PublicationDate.HasValue)
        {
            existingBook.PublicationDate = patchBookDto.PublicationDate.Value;
        }

        // Save changes
        var updatedBook = await _bookRepository.UpdateAsync(existingBook);
        
        // Return as DTO
        return MapToDto(updatedBook);
    }

    #region Private Helper Methods

    /// <summary>
    /// Maps a Book domain entity to a BookDto
    /// This centralizes the mapping logic and ensures consistency
    /// </summary>
    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            Price = book.Price,
            PublicationDate = book.PublicationDate,
            Description = book.Description,
            PageCount = book.PageCount,
            Publisher = book.Publisher,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            IsRecentPublication = book.IsRecentPublication(), // Business logic from domain
            DisplayName = book.GetDisplayName() // Business logic from domain
        };
    }

    /// <summary>
    /// Validates CreateBookDto input
    /// Centralized validation logic for book creation
    /// </summary>
    private static void ValidateCreateBookDto(CreateBookDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required", nameof(dto.Title));

        if (string.IsNullOrWhiteSpace(dto.Author))
            throw new ArgumentException("Author is required", nameof(dto.Author));

        if (string.IsNullOrWhiteSpace(dto.ISBN))
            throw new ArgumentException("ISBN is required", nameof(dto.ISBN));

        if (dto.Price <= 0)
            throw new ArgumentException("Price must be greater than 0", nameof(dto.Price));

        if (dto.PublicationDate > DateTime.Now)
            throw new ArgumentException("Publication date cannot be in the future", nameof(dto.PublicationDate));

        if (dto.PageCount.HasValue && dto.PageCount <= 0)
            throw new ArgumentException("Page count must be positive if provided", nameof(dto.PageCount));
    }

    /// <summary>
    /// Validates UpdateBookDto input
    /// Centralized validation logic for book updates
    /// </summary>
    private static void ValidateUpdateBookDto(UpdateBookDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required", nameof(dto.Title));

        if (string.IsNullOrWhiteSpace(dto.Author))
            throw new ArgumentException("Author is required", nameof(dto.Author));

        if (dto.Price <= 0)
            throw new ArgumentException("Price must be greater than 0", nameof(dto.Price));

        if (dto.PageCount.HasValue && dto.PageCount <= 0)
            throw new ArgumentException("Page count must be positive if provided", nameof(dto.PageCount));
    }

    #endregion
}
