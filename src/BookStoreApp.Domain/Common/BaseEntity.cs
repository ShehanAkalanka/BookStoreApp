namespace BookStoreApp.Domain.Common;

/// <summary>
/// Base entity class that provides common properties for all domain entities
/// This follows the DRY principle and ensures consistency across all entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity
    /// Using Guid provides better distribution and security than auto-incrementing integers
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created
    /// Useful for auditing and tracking when records were added
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the entity was last updated
    /// Helps track when data was modified, useful for caching and sync scenarios
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Constructor that initializes the entity with default values
    /// This ensures every entity has a unique ID and proper timestamps from creation
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid(); // Generate a new unique identifier
        CreatedAt = DateTime.UtcNow; // Use UTC to avoid timezone issues
        UpdatedAt = DateTime.UtcNow; // Initialize update time same as creation time
    }

    /// <summary>
    /// Method to update the UpdatedAt timestamp
    /// Call this method whenever the entity is modified
    /// </summary>
    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
