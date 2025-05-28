using BookStoreApp.Domain.Entities;
using BookStoreApp.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Infrastructure.Data;

/// <summary>
/// Database context for the BookStore application
/// This class configures Entity Framework and defines the database schema
/// It serves as the bridge between our domain entities and the database
/// </summary>
public class BookStoreDbContext : DbContext
{
    /// <summary>
    /// Constructor that accepts DbContext options
    /// This allows for dependency injection and configuration in startup
    /// </summary>
    /// <param name="options">Database context options including connection string</param>
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// DbSet for Book entities
    /// This represents the Books table in the database
    /// Entity Framework uses this to generate SQL queries
    /// </summary>
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// Configures the entity mappings and database schema
    /// This method is called by Entity Framework to build the model
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure entities</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Call base implementation
        base.OnModelCreating(modelBuilder);

        // Configure Book entity
        ConfigureBookEntity(modelBuilder);
    }

    /// <summary>
    /// Configures the Book entity mapping to database table
    /// This defines how our domain entity maps to database columns
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure the Book entity</param>
    private static void ConfigureBookEntity(ModelBuilder modelBuilder)
    {
        // Configure the Book entity
        var bookEntity = modelBuilder.Entity<Book>();

        // Configure the table name
        bookEntity.ToTable("Books");

        // Configure primary key
        bookEntity.HasKey(b => b.Id);

        // Configure Title property
        bookEntity.Property(b => b.Title)
            .IsRequired() // Cannot be null
            .HasMaxLength(200); // Business rule: max 200 characters

        // Configure Author property
        bookEntity.Property(b => b.Author)
            .IsRequired() // Cannot be null
            .HasMaxLength(100); // Business rule: max 100 characters

        // Configure ISBN property
        bookEntity.Property(b => b.ISBN)
            .IsRequired() // Cannot be null
            .HasMaxLength(20); // Standard ISBN length

        // Create unique index on ISBN (business rule: ISBN must be unique)
        bookEntity.HasIndex(b => b.ISBN)
            .IsUnique()
            .HasDatabaseName("IX_Books_ISBN");

        // Configure Price property
        bookEntity.Property(b => b.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)"); // Precision for currency

        // Configure PublicationDate
        bookEntity.Property(b => b.PublicationDate)
            .IsRequired()
            .HasColumnType("datetime2"); // Use datetime2 for better precision

        // Configure optional Description
        bookEntity.Property(b => b.Description)
            .HasMaxLength(1000); // Reasonable limit for description

        // Configure optional PageCount
        bookEntity.Property(b => b.PageCount)
            .IsRequired(false); // Nullable

        // Configure optional Publisher
        bookEntity.Property(b => b.Publisher)
            .HasMaxLength(100); // Reasonable limit for publisher name

        // Configure audit fields from BaseEntity
        bookEntity.Property(b => b.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        bookEntity.Property(b => b.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Add indexes for common query patterns
        bookEntity.HasIndex(b => b.Title)
            .HasDatabaseName("IX_Books_Title");

        bookEntity.HasIndex(b => b.Author)
            .HasDatabaseName("IX_Books_Author");

        bookEntity.HasIndex(b => b.PublicationDate)
            .HasDatabaseName("IX_Books_PublicationDate");

        // Composite index for search functionality
        bookEntity.HasIndex(b => new { b.Title, b.Author })
            .HasDatabaseName("IX_Books_Title_Author");
    }

    /// <summary>
    /// Override SaveChanges to automatically handle audit fields
    /// This ensures UpdatedAt is always set when entities are modified
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically handle audit fields
    /// This ensures UpdatedAt is always set when entities are modified
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates audit fields for modified entities
    /// This automatically sets UpdatedAt timestamp when entities are changed
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                // For new entities, set both created and updated timestamps
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                // For modified entities, only update the UpdatedAt timestamp
                // Prevent CreatedAt from being modified
                entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
