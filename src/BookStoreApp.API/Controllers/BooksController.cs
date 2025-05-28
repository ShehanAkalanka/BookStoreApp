using BookStoreApp.Application.DTOs;
using BookStoreApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.API.Controllers;

/// <summary>
/// API Controller for book-related operations
/// This controller handles HTTP requests for book management
/// Follows RESTful API conventions and returns appropriate HTTP status codes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    // Service dependency for business logic
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    /// <summary>
    /// Constructor with dependency injection
    /// </summary>
    /// <param name="bookService">Service for book business logic</param>
    /// <param name="logger">Logger for request tracking and debugging</param>
    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all books
    /// GET api/books
    /// </summary>
    /// <returns>List of all books</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
    {
        try
        {
            _logger.LogInformation("Getting all books");
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all books");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Gets a specific book by ID
    /// GET api/books/{id}
    /// </summary>
    /// <param name="id">The unique identifier of the book</param>
    /// <returns>Book details if found</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetBook(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting book with ID: {BookId}", id);
            var book = await _bookService.GetBookByIdAsync(id);
            
            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found", id);
                return NotFound($"Book with ID '{id}' not found");
            }

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting book with ID: {BookId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Gets a book by ISBN
    /// GET api/books/isbn/{isbn}
    /// </summary>
    /// <param name="isbn">The ISBN of the book</param>
    /// <returns>Book details if found</returns>
    [HttpGet("isbn/{isbn}")]
    public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
    {
        try
        {
            _logger.LogInformation("Getting book with ISBN: {ISBN}", isbn);
            var book = await _bookService.GetBookByIsbnAsync(isbn);
            
            if (book == null)
            {
                _logger.LogWarning("Book with ISBN {ISBN} not found", isbn);
                return NotFound($"Book with ISBN '{isbn}' not found");
            }

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting book with ISBN: {ISBN}", isbn);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Creates a new book
    /// POST api/books
    /// </summary>
    /// <param name="createBookDto">Book creation data</param>
    /// <returns>Created book details</returns>
    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        try
        {
            _logger.LogInformation("Creating new book with title: {Title}", createBookDto?.Title);
            
            if (createBookDto == null)
            {
                return BadRequest("Book data is required");
            }

            var createdBook = await _bookService.CreateBookAsync(createBookDto);
            
            // Return 201 Created with location header pointing to the new resource
            return CreatedAtAction(
                nameof(GetBook), 
                new { id = createdBook.Id }, 
                createdBook);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error while creating book");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while creating book");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating book");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Updates an existing book
    /// PUT api/books/{id}
    /// </summary>
    /// <param name="id">The ID of the book to update</param>
    /// <param name="updateBookDto">Updated book data</param>
    /// <returns>Updated book details</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookDto>> UpdateBook(Guid id, [FromBody] UpdateBookDto updateBookDto)
    {
        try
        {
            _logger.LogInformation("Updating book with ID: {BookId}", id);
            
            if (updateBookDto == null)
            {
                return BadRequest("Book data is required");
            }

            var updatedBook = await _bookService.UpdateBookAsync(id, updateBookDto);
            return Ok(updatedBook);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error while updating book with ID: {BookId}", id);
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Book with ID {BookId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating book with ID: {BookId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Deletes a book
    /// DELETE api/books/{id}
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteBook(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting book with ID: {BookId}", id);
            
            var deleted = await _bookService.DeleteBookAsync(id);
            
            if (!deleted)
            {
                _logger.LogWarning("Book with ID {BookId} not found for deletion", id);
                return NotFound($"Book with ID '{id}' not found");
            }

            return NoContent(); // 204 No Content - successful deletion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting book with ID: {BookId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Searches books by title or author
    /// GET api/books/search?term={searchTerm}
    /// </summary>
    /// <param name="term">Search term to match against title or author</param>
    /// <returns>List of matching books</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string term)
    {
        try
        {
            _logger.LogInformation("Searching books with term: {SearchTerm}", term);
            
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required");
            }

            var books = await _bookService.SearchBooksAsync(term);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching books with term: {SearchTerm}", term);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Gets books published within a date range
    /// GET api/books/date-range?startDate={start}&endDate={end}
    /// </summary>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <returns>List of books published within the date range</returns>
    [HttpGet("date-range")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Getting books published between {StartDate} and {EndDate}", 
                startDate, endDate);
            
            var books = await _bookService.GetBooksByDateRangeAsync(startDate, endDate);
            return Ok(books);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid date range provided");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting books by date range");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Partially updates a book (PATCH operation)
    /// Only updates the fields provided in the request body
    /// PATCH api/books/{id}
    /// </summary>
    /// <param name="id">The ID of the book to update</param>
    /// <param name="patchBookDto">Partial book data to update</param>
    /// <returns>Updated book details</returns>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<BookDto>> PatchBook(Guid id, [FromBody] PatchBookDto patchBookDto)
    {
        try
        {
            _logger.LogInformation("Attempting to partially update book with ID: {BookId}", id);

            // Validate that at least one field is provided for update
            if (IsEmptyPatch(patchBookDto))
            {
                _logger.LogWarning("Empty patch request received for book ID: {BookId}", id);
                return BadRequest("At least one field must be provided for update");
            }

            var result = await _bookService.PatchBookAsync(id, patchBookDto);
            
            if (result == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found for partial update", id);
                return NotFound($"Book with ID {id} not found");
            }

            _logger.LogInformation("Successfully partially updated book with ID: {BookId}", id);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid input for partial book update. BookId: {BookId}", id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation while partially updating book. BookId: {BookId}", id);
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while partially updating book. BookId: {BookId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Helper method to check if the patch DTO is empty
    /// </summary>
    private static bool IsEmptyPatch(PatchBookDto patchDto)
    {
        return patchDto.Title == null &&
               patchDto.Author == null &&
               patchDto.ISBN == null &&
               patchDto.Price == null &&
               patchDto.Description == null &&
               patchDto.PageCount == null &&
               patchDto.Publisher == null &&
               patchDto.PublicationDate == null;
    }
}
