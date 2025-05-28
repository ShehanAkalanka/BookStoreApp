using BookStoreApp.Domain.Entities;
using BookStoreApp.Infrastructure.Data;

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
                Description = "Even bad code can function. But if code isn't clean, it can bring a development organization to its knees. Every year, countless hours and significant resources are lost because of poorly written code.",
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
                Description = "Capturing a wealth of experience about the design of object-oriented software, four top-notch designers present a catalog of simple and succinct solutions to commonly occurring design problems.",
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
                Description = "The Pragmatic Programmer is one of those rare tech books you'll read, re-read, and read to others over the years. Whether you're new to the field or an experienced practitioner, you'll come away with fresh insights each and every time.",
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
                Description = "Eric Evans has written a fantastic book on how you can make the design of your software match your mental model of the problem domain you are addressing.",
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
                Description = "Refactoring is about improving the design of existing code. It is the process of changing a software system in such a way that it does not alter the external behavior of the code, yet improves its internal structure.",
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
                Description = "By applying universal rules of software architecture, you can dramatically improve developer productivity throughout the life of any software system.",
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
                Description = "No matter how much experience you have with JavaScript, odds are you don't fully understand the language. This concise yet in-depth guide takes you inside scope and closures, two core concepts you need to know to become a more efficient and effective JavaScript programmer.",
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
                Description = "The practice of enterprise application development has benefited from the emergence of many new enabling technologies. Multi-tiered object-oriented platforms, such as Java and .NET, have become commonplace.",
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
                Description = "Quite simply, test-driven development is meant to eliminate fear in application development. While some fear is healthy (often viewed as a conscience that tells programmers to 'be careful!'), the author believes that byproducts of fear include tentative, grumpy, and uncommunicative programmers who are unable to absorb constructive criticism.",
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
                Description = "The microservice architecture is rapidly gaining ground in the industry as a viable alternative to monolithic applications and service-oriented architectures. Because this architecture is still evolving, there's a lot of confusion about how to use it and when it makes sense.",
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
