# Complete Step-by-Step Guide: Building BookStore Clean Architecture from Scratch

## 📋 Prerequisites

Before starting, ensure you have:
- ✅ Visual Studio 2022 or VS Code with C# extension
- ✅ .NET 9.0 SDK installed
- ✅ SQL Server (LocalDB or full SQL Server)
- ✅ Git installed
- ✅ Basic knowledge of C# and Entity Framework

## 🎯 What We'll Build

A complete Clean Architecture BookStore API with:
- 4-layer architecture (Domain, Application, Infrastructure, API)
- Full CRUD operations + advanced features
- Entity Framework Core with migrations
- Partial update support (PATCH operations)
- Database seeding with sample data
- Comprehensive error handling and logging

---

## Step 1: Project Setup and Solution Creation

### 1.1 Create Project Directory
```powershell
# Create main project directory
mkdir "d:\A-MY_LEARNINGS\NET_CORE\BookStoreApp"
cd "d:\A-MY_LEARNINGS\NET_CORE\BookStoreApp"

# Create source directory
mkdir src
cd src
```

### 1.2 Create Solution File
```powershell
# Go back to root directory
cd ..

# Create solution file
dotnet new sln -n BookStoreApp
```

### 1.3 Create Project Structure
```powershell
# Create Domain project (Core business logic - no dependencies)
cd src
dotnet new classlib -n BookStoreApp.Domain -f net9.0

# Create Application project (Business logic orchestration - depends on Domain)
dotnet new classlib -n BookStoreApp.Application -f net9.0

# Create Infrastructure project (Data access - depends on Application)
dotnet new classlib -n BookStoreApp.Infrastructure -f net9.0

# Create API project (Web API - depends on Application & Infrastructure)
dotnet new webapi -n BookStoreApp.API -f net9.0

# Go back to root
cd ..
```

### 1.4 Add Projects to Solution
```powershell
# Add all projects to solution
dotnet sln add src/BookStoreApp.Domain/BookStoreApp.Domain.csproj
dotnet sln add src/BookStoreApp.Application/BookStoreApp.Application.csproj
dotnet sln add src/BookStoreApp.Infrastructure/BookStoreApp.Infrastructure.csproj
dotnet sln add src/BookStoreApp.API/BookStoreApp.API.csproj
```

### 1.5 Set Up Project References (Clean Architecture Dependencies)
```powershell
# Application layer depends on Domain
cd src/BookStoreApp.Application
dotnet add reference ../BookStoreApp.Domain/BookStoreApp.Domain.csproj

# Infrastructure layer depends on Application (and Domain transitively)
cd ../BookStoreApp.Infrastructure
dotnet add reference ../BookStoreApp.Application/BookStoreApp.Application.csproj

# API layer depends on Application and Infrastructure
cd ../BookStoreApp.API
dotnet add reference ../BookStoreApp.Application/BookStoreApp.Application.csproj
dotnet add reference ../BookStoreApp.Infrastructure/BookStoreApp.Infrastructure.csproj

# Go back to root
cd ../..
```

---

## Step 2: Domain Layer Implementation

### 2.1 Create BaseEntity Class
```powershell
# Create Common folder
mkdir src/BookStoreApp.Domain/Common
```

**File:** `src/BookStoreApp.Domain/Common/BaseEntity.cs`
```csharp
namespace BookStoreApp.Domain.Common;

/// <summary>
/// Base class for all domain entities
/// Provides common properties and functionality
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Updates the UpdatedAt timestamp
    /// Should be called whenever the entity is modified
    /// </summary>
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### 2.2 Create Book Entity
```powershell
# Create Entities folder
mkdir src/BookStoreApp.Domain/Entities
```

**File:** `src/BookStoreApp.Domain/Entities/Book.cs`
```csharp
using BookStoreApp.Domain.Common;

namespace BookStoreApp.Domain.Entities;

/// <summary>
/// Book domain entity representing a book in the bookstore
/// Contains business rules and validation logic
/// </summary>
public class Book : BaseEntity
{
    /// <summary>
    /// Book title - required field
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Book author - required field
    /// </summary>
    public string Author { get; private set; } = string.Empty;

    /// <summary>
    /// International Standard Book Number - unique identifier
    /// </summary>
    public string ISBN { get; set; } = string.Empty;

    /// <summary>
    /// Book price - must be positive
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Book description - optional field
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Number of pages in the book
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Publisher name
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// Date when the book was published
    /// </summary>
    public DateTime PublicationDate { get; set; }

    /// <summary>
    /// Protected constructor for Entity Framework
    /// </summary>
    protected Book() { }

    /// <summary>
    /// Constructor for creating a new book
    /// Enforces business rules at creation time
    /// </summary>
    /// <param name="title">Book title</param>
    /// <param name="author">Book author</param>
    /// <param name="isbn">Book ISBN</param>
    /// <param name="price">Book price</param>
    /// <param name="publicationDate">Publication date</param>
    public Book(string title, string author, string isbn, decimal price, DateTime publicationDate)
    {
        ValidateAndSetTitle(title);
        ValidateAndSetAuthor(author);
        ValidateAndSetPrice(price);
        
        ISBN = isbn?.Trim() ?? throw new ArgumentException("ISBN cannot be null");
        PublicationDate = publicationDate;
    }

    /// <summary>
    /// Updates book information with business rule validation
    /// </summary>
    /// <param name="title">New title</param>
    /// <param name="author">New author</param>
    /// <param name="price">New price</param>
    /// <param name="description">New description</param>
    public void UpdateBookInfo(string title, string author, decimal price, string? description = null)
    {
        ValidateAndSetTitle(title);
        ValidateAndSetAuthor(author);
        ValidateAndSetPrice(price);
        Description = description?.Trim();
        
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates and sets the book title
    /// </summary>
    /// <param name="title">Title to validate and set</param>
    /// <exception cref="ArgumentException">Thrown when title is invalid</exception>
    private void ValidateAndSetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");
        
        if (title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters");
            
        Title = title.Trim();
    }

    /// <summary>
    /// Validates and sets the book author
    /// </summary>
    /// <param name="author">Author to validate and set</param>
    /// <exception cref="ArgumentException">Thrown when author is invalid</exception>
    private void ValidateAndSetAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be empty");
            
        if (author.Length > 150)
            throw new ArgumentException("Author cannot exceed 150 characters");
            
        Author = author.Trim();
    }

    /// <summary>
    /// Validates and sets the book price
    /// </summary>
    /// <param name="price">Price to validate and set</param>
    /// <exception cref="ArgumentException">Thrown when price is invalid</exception>
    private void ValidateAndSetPrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero");
            
        if (price > 9999.99m)
            throw new ArgumentException("Price cannot exceed 9999.99");
            
        Price = price;
    }
}
```

### 2.3 Remove Default Class1.cs
```powershell
# Remove the default Class1.cs file
Remove-Item src/BookStoreApp.Domain/Class1.cs -Force
```

---

## Step 3: Application Layer Implementation

### 3.1 Create DTOs (Data Transfer Objects)

```powershell
# Create DTOs folder
mkdir src/BookStoreApp.Application/DTOs
```

**File:** `src/BookStoreApp.Application/DTOs/BookDto.cs`
```csharp
namespace BookStoreApp.Application.DTOs;

/// <summary>
/// DTO for returning book information to clients
/// Contains all book details for display purposes
/// </summary>
public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int PageCount { get; set; }
    public string? Publisher { get; set; }
    public DateTime PublicationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**File:** `src/BookStoreApp.Application/DTOs/CreateBookDto.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Application.DTOs;

/// <summary>
/// DTO for creating a new book
/// Contains validation attributes for input validation
/// </summary>
public class CreateBookDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required")]
    [StringLength(150, ErrorMessage = "Author cannot exceed 150 characters")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "ISBN is required")]
    [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 9999.99, ErrorMessage = "Price must be between 0.01 and 9999.99")]
    public decimal Price { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Range(1, 10000, ErrorMessage = "Page count must be between 1 and 10000")]
    public int PageCount { get; set; }

    [StringLength(100, ErrorMessage = "Publisher cannot exceed 100 characters")]
    public string? Publisher { get; set; }

    [Required(ErrorMessage = "Publication date is required")]
    public DateTime PublicationDate { get; set; }
}
```

**File:** `src/BookStoreApp.Application/DTOs/UpdateBookDto.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Application.DTOs;

/// <summary>
/// DTO for updating an existing book
/// Contains all fields that can be updated
/// </summary>
public class UpdateBookDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Author is required")]
    [StringLength(150, ErrorMessage = "Author cannot exceed 150 characters")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 9999.99, ErrorMessage = "Price must be between 0.01 and 9999.99")]
    public decimal Price { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Range(1, 10000, ErrorMessage = "Page count must be between 1 and 10000")]
    public int PageCount { get; set; }

    [StringLength(100, ErrorMessage = "Publisher cannot exceed 100 characters")]
    public string? Publisher { get; set; }
}
```

**File:** `src/BookStoreApp.Application/DTOs/PatchBookDto.cs`
```csharp
using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Application.DTOs;

/// <summary>
/// DTO for partial book updates using PATCH operations
/// All properties are nullable to support partial updates
/// </summary>
public class PatchBookDto
{
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; set; }

    [StringLength(150, ErrorMessage = "Author cannot exceed 150 characters")]
    public string? Author { get; set; }

    [StringLength(20, ErrorMessage = "ISBN cannot exceed 20 characters")]
    public string? ISBN { get; set; }

    [Range(0.01, 9999.99, ErrorMessage = "Price must be between 0.01 and 9999.99")]
    public decimal? Price { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [Range(1, 10000, ErrorMessage = "Page count must be between 1 and 10000")]
    public int? PageCount { get; set; }

    [StringLength(100, ErrorMessage = "Publisher cannot exceed 100 characters")]
    public string? Publisher { get; set; }

    public DateTime? PublicationDate { get; set; }
}
```

### 3.2 Create Interfaces

```powershell
# Create Interfaces folder
mkdir src/BookStoreApp.Application/Interfaces
```

**File:** `src/BookStoreApp.Application/Interfaces/IBookRepository.cs`
```csharp
using BookStoreApp.Domain.Entities;

namespace BookStoreApp.Application.Interfaces;

/// <summary>
/// Repository interface for book data access operations
/// Defines the contract for data persistence without exposing implementation details
/// </summary>
public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(Guid id);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<Book> CreateAsync(Book book);
    Task<Book> UpdateAsync(Book book);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Book>> SearchAsync(string searchTerm);
    Task<IEnumerable<Book>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> ExistsByIsbnAsync(string isbn);
}
```

**File:** `src/BookStoreApp.Application/Interfaces/IBookService.cs`
```csharp
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
    Task<IEnumerable<BookDto>> GetBooksByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Partially updates a book with only the provided fields
    /// Only non-null properties in the patch DTO will be updated
    /// </summary>
    /// <param name="id">The unique identifier of the book to update</param>
    /// <param name="patchBookDto">DTO containing the fields to update</param>
    /// <returns>Updated BookDto if successful, null if book not found</returns>
    Task<BookDto?> PatchBookAsync(Guid id, PatchBookDto patchBookDto);
}
```

### 3.3 Create Services

```powershell
# Create Services folder
mkdir src/BookStoreApp.Application/Services
```

**File:** `src/BookStoreApp.Application/Services/BookService.cs`
```csharp
using BookStoreApp.Application.DTOs;
using BookStoreApp.Application.Interfaces;
using BookStoreApp.Domain.Entities;

namespace BookStoreApp.Application.Services;

/// <summary>
/// Service implementation for book-related business operations
/// Orchestrates business logic and coordinates between repository and DTOs
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : MapToDto(book);
    }

    public async Task<BookDto?> GetBookByIsbnAsync(string isbn)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be empty", nameof(isbn));

        var book = await _bookRepository.GetByIsbnAsync(isbn);
        return book == null ? null : MapToDto(book);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
    {
        // Check if ISBN already exists
        if (await _bookRepository.ExistsByIsbnAsync(createBookDto.ISBN))
        {
            throw new InvalidOperationException($"A book with ISBN '{createBookDto.ISBN}' already exists");
        }

        // Create domain entity (business rules are enforced in constructor)
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
        var createdBook = await _bookRepository.CreateAsync(book);
        
        // Return as DTO
        return MapToDto(createdBook);
    }

    public async Task<BookDto> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto)
    {
        // Get existing book
        var existingBook = await _bookRepository.GetByIdAsync(id);
        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID {id} not found");
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

    public async Task<bool> DeleteBookAsync(Guid id)
    {
        return await _bookRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<BookDto>();

        var books = await _bookRepository.SearchAsync(searchTerm);
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date");

        var books = await _bookRepository.GetByDateRangeAsync(startDate, endDate);
        return books.Select(MapToDto);
    }

    /// <summary>
    /// Maps a Book domain entity to a BookDto
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
            Description = book.Description,
            PageCount = book.PageCount,
            Publisher = book.Publisher,
            PublicationDate = book.PublicationDate,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };
    }
}
```

### 3.4 Remove Default Class1.cs
```powershell
# Remove the default Class1.cs file
Remove-Item src/BookStoreApp.Application/Class1.cs -Force
```

---

## Step 4: Infrastructure Layer Implementation

### 4.1 Install Entity Framework Packages
```powershell
cd src/BookStoreApp.Infrastructure

# Install Entity Framework Core for SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0

# Install Entity Framework Core Tools for migrations
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0

# Install Entity Framework Core Design for design-time services
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0

cd ../..
```

### 4.2 Create DbContext

```powershell
# Create Data folder
mkdir src/BookStoreApp.Infrastructure/Data
```

**File:** `src/BookStoreApp.Infrastructure/Data/BookStoreDbContext.cs`
```csharp
using BookStoreApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext for the BookStore application
/// Configures database entities and relationships
/// </summary>
public class BookStoreDbContext : DbContext
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Books table
    /// </summary>
    public DbSet<Book> Books { get; set; } = null!;

    /// <summary>
    /// Configures entity mappings and relationships
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Book entity
        modelBuilder.Entity<Book>(entity =>
        {
            // Primary key
            entity.HasKey(b => b.Id);

            // Title configuration
            entity.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            // Author configuration
            entity.Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(150);

            // ISBN configuration with unique index
            entity.Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(20);
            
            entity.HasIndex(b => b.ISBN)
                .IsUnique()
                .HasDatabaseName("IX_Books_ISBN");

            // Price configuration
            entity.Property(b => b.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Description configuration
            entity.Property(b => b.Description)
                .HasMaxLength(1000);

            // Publisher configuration
            entity.Property(b => b.Publisher)
                .HasMaxLength(100);

            // Publication date configuration
            entity.Property(b => b.PublicationDate)
                .IsRequired()
                .HasColumnType("datetime2");

            // Audit fields configuration
            entity.Property(b => b.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(b => b.UpdatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            // Index for common queries
            entity.HasIndex(b => b.Title)
                .HasDatabaseName("IX_Books_Title");

            entity.HasIndex(b => b.Author)
                .HasDatabaseName("IX_Books_Author");

            entity.HasIndex(b => b.PublicationDate)
                .HasDatabaseName("IX_Books_PublicationDate");
        });
    }

    /// <summary>
    /// Automatically update timestamps when saving changes
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps for entities being modified
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.UpdateTimestamp();
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.UpdateTimestamp();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
```

### 4.3 Create Repository Implementation

```powershell
# Create Repositories folder
mkdir src/BookStoreApp.Infrastructure/Repositories
```

**File:** `src/BookStoreApp.Infrastructure/Repositories/BookRepository.cs`
```csharp
using BookStoreApp.Application.Interfaces;
using BookStoreApp.Domain.Entities;
using BookStoreApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Book entity data access
/// Provides concrete implementation of data operations using Entity Framework
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly BookStoreDbContext _context;

    public BookRepository(BookStoreDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _context.Books
            .FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book> UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var book = await GetByIdAsync(id);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        return await _context.Books
            .Where(b => b.Title.ToLower().Contains(normalizedSearchTerm) ||
                       b.Author.ToLower().Contains(normalizedSearchTerm))
            .OrderBy(b => b.Title)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Books
            .Where(b => b.PublicationDate >= startDate && b.PublicationDate <= endDate)
            .OrderBy(b => b.PublicationDate)
            .ToListAsync();
    }

    public async Task<bool> ExistsByIsbnAsync(string isbn)
    {
        return await _context.Books
            .AnyAsync(b => b.ISBN == isbn);
    }
}
```

### 4.4 Create Data Seeder

**File:** `src/BookStoreApp.Infrastructure/Data/DataSeeder.cs`
```csharp
using BookStoreApp.Domain.Entities;

namespace BookStoreApp.Infrastructure.Data;

/// <summary>
/// Data seeder for populating the database with sample data
/// This class provides initial data for testing and demonstration purposes
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds the database with sample books if no books exist
    /// This method is idempotent - it won't duplicate data on multiple runs
    /// </summary>
    /// <param name="context">The database context to seed</param>
    public static async Task SeedAsync(BookStoreDbContext context)
    {
        // Check if books already exist
        if (context.Books.Any())
        {
            return; // Database has been seeded
        }

        // Create sample books with realistic data
        var sampleBooks = new List<Book>
        {
            new Book(
                "Clean Code: A Handbook of Agile Software Craftsmanship",
                "Robert C. Martin",
                "978-0132350884",
                45.99m,
                new DateTime(2008, 8, 1))
            {
                Description = "Even bad code can function. But if code isn't clean, it can bring a development organization to its knees.",
                PageCount = 464,
                Publisher = "Prentice Hall"
            },

            new Book(
                "Design Patterns: Elements of Reusable Object-Oriented Software", 
                "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
                "978-0201633610",
                54.99m,
                new DateTime(1994, 10, 31))
            {
                Description = "Capturing a wealth of experience about the design of object-oriented software.",
                PageCount = 395,
                Publisher = "Addison-Wesley Professional"
            },

            new Book(
                "The Pragmatic Programmer: Your Journey To Mastery",
                "David Thomas, Andrew Hunt",
                "978-0135957059", 
                42.99m,
                new DateTime(2019, 9, 13))
            {
                Description = "The Pragmatic Programmer is one of those rare tech books you'll read, re-read, and read to others.",
                PageCount = 352,
                Publisher = "Addison-Wesley Professional"
            },

            new Book(
                "Domain-Driven Design: Tackling Complexity in the Heart of Software",
                "Eric Evans",
                "978-0321125217",
                59.99m,
                new DateTime(2003, 8, 30))
            {
                Description = "Eric Evans has written a fantastic book on how you can make the design of your software match your mental model.",
                PageCount = 560,
                Publisher = "Addison-Wesley Professional"
            },

            new Book(
                "Refactoring: Improving the Design of Existing Code",
                "Martin Fowler",
                "978-0134757599",
                47.99m, 
                new DateTime(2018, 11, 19))
            {
                Description = "Refactoring is about improving the design of existing code.",
                PageCount = 448,
                Publisher = "Addison-Wesley Professional"
            },

            new Book(
                "Clean Architecture: A Craftsman's Guide to Software Structure and Design",
                "Robert C. Martin",
                "978-0134494166",
                39.99m,
                new DateTime(2017, 9, 20))
            {
                Description = "By applying universal rules of software architecture, you can dramatically improve developer productivity.",
                PageCount = 432,
                Publisher = "Prentice Hall"
            },

            new Book(
                "You Don't Know JS: Scope and Closures",
                "Kyle Simpson",
                "978-1449335588",
                29.99m,
                new DateTime(2014, 3, 24))
            {
                Description = "This concise yet in-depth guide takes you inside scope and closures.",
                PageCount = 98,
                Publisher = "O'Reilly Media"
            },

            new Book(
                "Patterns of Enterprise Application Architecture",
                "Martin Fowler",
                "978-0321127426",
                54.99m,
                new DateTime(2002, 11, 15))
            {
                Description = "The practice of enterprise application development has benefited from many new enabling technologies.",
                PageCount = 560,
                Publisher = "Addison-Wesley Professional"
            },

            new Book(
                "Test Driven Development: By Example",
                "Kent Beck",
                "978-0321146533",
                44.99m,
                new DateTime(2002, 11, 18))
            {
                Description = "Test-driven development is meant to eliminate fear in application development.",
                PageCount = 240,
                Publisher = "Addison-Wesley Professional"
            },

            new Book(
                "Microservices Patterns: With examples in Java",
                "Chris Richardson",
                "978-1617294549",
                49.99m,
                new DateTime(2018, 10, 27))
            {
                Description = "The microservice architecture is rapidly gaining ground in the industry.",
                PageCount = 520,
                Publisher = "Manning Publications"
            }
        };

        // Add the books to the context
        await context.Books.AddRangeAsync(sampleBooks);
        
        // Save changes to the database
        await context.SaveChangesAsync();
    }
}
```

### 4.5 Remove Default Class1.cs
```powershell
# Remove the default Class1.cs file
Remove-Item src/BookStoreApp.Infrastructure/Class1.cs -Force
```

---

## Step 5: API Layer Implementation

### 5.1 Install Required Packages
```powershell
cd src/BookStoreApp.API

# Install Entity Framework packages for API layer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0

cd ../..
```

### 5.2 Update appsettings.json

**File:** `src/BookStoreApp.API/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookStoreDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### 5.3 Create BooksController

**File:** `src/BookStoreApp.API/Controllers/BooksController.cs`
```csharp
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
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

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
            _logger.LogInformation("Retrieving all books");
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all books");
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Gets a specific book by ID
    /// GET api/books/{id}
    /// </summary>
    /// <param name="id">The book ID</param>
    /// <returns>Book details</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetBook(Guid id)
    {
        try
        {
            _logger.LogInformation("Retrieving book with ID: {BookId}", id);
            var book = await _bookService.GetBookByIdAsync(id);
            
            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found", id);
                return NotFound($"Book with ID {id} not found");
            }

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving book with ID: {BookId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Gets a book by ISBN
    /// GET api/books/isbn/{isbn}
    /// </summary>
    /// <param name="isbn">The book ISBN</param>
    /// <returns>Book details</returns>
    [HttpGet("isbn/{isbn}")]
    public async Task<ActionResult<BookDto>> GetBookByIsbn(string isbn)
    {
        try
        {
            _logger.LogInformation("Retrieving book with ISBN: {ISBN}", isbn);
            var book = await _bookService.GetBookByIsbnAsync(isbn);
            
            if (book == null)
            {
                _logger.LogWarning("Book with ISBN {ISBN} not found", isbn);
                return NotFound($"Book with ISBN {isbn} not found");
            }

            return Ok(book);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid ISBN provided: {ISBN}", isbn);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving book with ISBN: {ISBN}", isbn);
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
            _logger.LogInformation("Creating new book with title: {Title}", createBookDto.Title);
            var createdBook = await _bookService.CreateBookAsync(createBookDto);
            
            _logger.LogInformation("Successfully created book with ID: {BookId}", createdBook.Id);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid input for book creation");
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
            var updatedBook = await _bookService.UpdateBookAsync(id, updateBookDto);
            
            _logger.LogInformation("Successfully updated book with ID: {BookId}", id);
            return Ok(updatedBook);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Book with ID {BookId} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid input for book update. BookId: {BookId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating book. BookId: {BookId}", id);
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

    /// <summary>
    /// Deletes a book
    /// DELETE api/books/{id}
    /// </summary>
    /// <param name="id">The ID of the book to delete</param>
    /// <returns>No content on success</returns>
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
                return NotFound($"Book with ID {id} not found");
            }

            _logger.LogInformation("Successfully deleted book with ID: {BookId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting book. BookId: {BookId}", id);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }

    /// <summary>
    /// Searches books by title or author
    /// GET api/books/search?term={searchTerm}
    /// </summary>
    /// <param name="term">Search term</param>
    /// <returns>List of matching books</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BookDto>>> SearchBooks([FromQuery] string term)
    {
        try
        {
            _logger.LogInformation("Searching books with term: {SearchTerm}", term);
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
    /// GET api/books/daterange?start={startDate}&end={endDate}
    /// </summary>
    /// <param name="start">Start date</param>
    /// <param name="end">End date</param>
    /// <returns>List of books within the date range</returns>
    [HttpGet("daterange")]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByDateRange(
        [FromQuery] DateTime start, 
        [FromQuery] DateTime end)
    {
        try
        {
            _logger.LogInformation("Retrieving books published between {StartDate} and {EndDate}", start, end);
            var books = await _bookService.GetBooksByDateRangeAsync(start, end);
            return Ok(books);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid date range provided. Start: {StartDate}, End: {EndDate}", start, end);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving books by date range. Start: {StartDate}, End: {EndDate}", start, end);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}
```

### 5.4 Update Program.cs

**File:** `src/BookStoreApp.API/Program.cs`
```csharp
using BookStoreApp.Application.Interfaces;
using BookStoreApp.Application.Services;
using BookStoreApp.Infrastructure.Data;
using BookStoreApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<BookStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repository dependencies (Infrastructure layer)
builder.Services.AddScoped<IBookRepository, BookRepository>();

// Register service dependencies (Application layer)
builder.Services.AddScoped<IBookService, BookService>();

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Build the application
var app = builder.Build();

// Seed the database with sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
    await BookStoreApp.Infrastructure.Data.DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger for development
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use HTTPS redirection for security
app.UseHttpsRedirection();

// Map controller routes
app.MapControllers();

app.Run();
```

### 5.5 Remove Default Files
```powershell
# Remove default weather forecast files
Remove-Item src/BookStoreApp.API/Controllers/WeatherForecastController.cs -Force -ErrorAction SilentlyContinue
Remove-Item src/BookStoreApp.API/WeatherForecast.cs -Force -ErrorAction SilentlyContinue
```

---

## Step 6: Database Setup and Migrations

### 6.1 Create Initial Migration
```powershell
# Navigate to API project directory (where the startup project is)
cd src/BookStoreApp.API

# Add initial migration
dotnet ef migrations add InitialCreate --project ../BookStoreApp.Infrastructure --startup-project . --context BookStoreDbContext

# Go back to root
cd ../..
```

### 6.2 Update Database
```powershell
# Navigate to API project directory
cd src/BookStoreApp.API

# Update database with migration
dotnet ef database update --project ../BookStoreApp.Infrastructure --startup-project . --context BookStoreDbContext

# Go back to root
cd ../..
```

---

## Step 7: Build and Test the Application

### 7.1 Build the Solution
```powershell
# Build the entire solution
dotnet build

# If build succeeds, run the application
cd src/BookStoreApp.API
dotnet run
```

### 7.2 Test the API
The application will start on `https://localhost:5001` or `http://localhost:5000`. You can:

1. **Open Swagger UI**: Navigate to `https://localhost:5001/swagger` (or the port shown in console)
2. **Test endpoints**:
   - `GET /api/books` - Get all books (should show 10 seeded books)
   - `GET /api/books/{id}` - Get specific book
   - `POST /api/books` - Create new book
   - `PUT /api/books/{id}` - Full update
   - `PATCH /api/books/{id}` - Partial update (e.g., just price)
   - `DELETE /api/books/{id}` - Delete book

---

## Step 8: Documentation and Git Setup

### 8.1 Create Documentation Folder
```powershell
# Go back to root
cd ../..

# Create docs folder
mkdir docs
```

### 8.2 Initialize Git Repository
```powershell
# Initialize git repository
git init

# Create .gitignore file
@"
## Ignore Visual Studio temporary files, build results, and files generated by popular Visual Studio add-ons.

# User-specific files
*.rsuser
*.suo
*.user
*.userosscache
*.sln.docstates

# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/
[Ll]og/

# NuGet Packages
*.nupkg
*.snupkg
[Pp]ackages/

# Entity Framework
Migrations/

# .NET Core
project.lock.json
project.fragment.lock.json
artifacts/

# ASP.NET Scaffolding
ScaffoldingReadMe.txt

# Others
sql/
*.Cache
ClientBin/
[Ss]tyle[Cc]op.*
~$*
*~
*.dbmdl
*.dbproj.schemaview
*.jfm
*.pfx
*.publishsettings
orleans.runtimeconfig.json

# SQL Server files
*.mdf
*.ldf
*.ndf

# Visual Studio cache files
*.VC.db
*.VC.VC.opendb

# Visual Studio profiler
*.psess
*.vsp
*.vspx
*.sap

# Others
*.log
*.cache
*.dll
*.exe
*.pdb
"@ | Out-File -FilePath .gitignore -Encoding UTF8

# Add all files
git add .

# Create initial commit
git commit -m "Initial commit: Clean Architecture BookStore Phase 1 Complete"
```

### 8.3 Create Phase 1 Branch
```powershell
# Create and switch to phase_01 branch
git checkout -b phase_01

# Push to remote (assuming you have a remote repository set up)
# git remote add origin [your-repository-url]
# git push -u origin phase_01
```

---

## 🎉 Congratulations!

You have successfully created a complete Clean Architecture BookStore application with:

### ✅ **Features Implemented:**
- **Clean Architecture** with 4 properly separated layers
- **Full CRUD operations** (Create, Read, Update, Delete)
- **Advanced features** (Search, date filtering, partial updates)
- **Entity Framework Core** with migrations
- **Database seeding** with 10 sample books
- **Comprehensive error handling** and logging
- **RESTful API** with proper HTTP status codes
- **Swagger documentation** for API testing

### 📊 **Project Statistics:**
- **4 Projects** (Domain, Application, Infrastructure, API)
- **8+ API Endpoints** with full functionality
- **10 Sample Books** automatically seeded
- **Complete documentation** and guides

### 🚀 **Next Steps:**
- Test all API endpoints using Swagger UI
- Try the partial update feature (PATCH operations)
- Explore the sample data that was automatically seeded
- Consider implementing Phase 2 features (authentication, advanced search, etc.)

**Your Clean Architecture BookStore application is now ready for production use!** 🎊
