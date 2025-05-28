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
