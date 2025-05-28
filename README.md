# BookStore Clean Architecture Application

## Overview
This is a comprehensive BookStore application built using Clean Architecture principles. The project is designed to demonstrate industry-level .NET development practices with clear separation of concerns and maintainable code structure.

## Project Evolution - Phased Approach

### 🚀 Phase 1: Main - Basic Clean Architecture Setup (Current Phase)
- ✅ Basic project structure with Clean Architecture layers
- ✅ Entity classes (Book)
- ✅ Basic CRUD operations
- ✅ Dependency injection configuration
- ✅ Entity Framework DbContext setup
- ✅ REST API controllers

### 📋 Upcoming Phases
- **Phase 2**: FluentValidation integration
- **Phase 3**: CQRS pattern with MediatR
- **Phase 4**: Repository and Unit of Work patterns
- **Phase 5**: Global error handling and logging
- **Phase 6**: JWT authentication and authorization
- **Phase 7**: Swagger documentation
- **Phase 8**: Comprehensive testing

## Architecture Overview

This application follows the Clean Architecture pattern with the following layers:

### 1. Domain Layer (Core)
- **Purpose**: Contains business entities, value objects, enums, and business rules
- **Dependencies**: None (dependency-free)
- **Location**: `BookStoreApp.Domain`

### 2. Application Layer (Use Cases)
- **Purpose**: Contains application logic, interfaces, and DTOs
- **Dependencies**: Domain layer only
- **Location**: `BookStoreApp.Application`

### 3. Infrastructure Layer
- **Purpose**: Implements external concerns (database, external APIs, file system)
- **Dependencies**: Application and Domain layers
- **Location**: `BookStoreApp.Infrastructure`

### 4. Presentation Layer (API)
- **Purpose**: Handles HTTP requests, responses, and API concerns
- **Dependencies**: Application and Infrastructure layers
- **Location**: `BookStoreApp.API`

## Project Structure

```
BookStoreApp/
├── src/
│   ├── BookStoreApp.Domain/           # Core business logic
│   │   ├── Entities/                  # Business entities
│   │   ├── Enums/                     # Domain enumerations
│   │   └── Common/                    # Shared domain concerns
│   │
│   ├── BookStoreApp.Application/      # Application layer
│   │   ├── DTOs/                      # Data Transfer Objects
│   │   ├── Interfaces/                # Service contracts
│   │   ├── Services/                  # Business services
│   │   └── Common/                    # Shared application concerns
│   │
│   ├── BookStoreApp.Infrastructure/   # External concerns
│   │   ├── Data/                      # Database context and configurations
│   │   ├── Repositories/              # Data access implementations
│   │   └── Services/                  # External service implementations
│   │
│   └── BookStoreApp.API/              # Web API layer
│       ├── Controllers/               # API controllers
│       ├── Middleware/                # Custom middleware
│       └── Extensions/                # Service configuration extensions
│
├── tests/                             # Test projects (Future phases)
└── docs/                              # Documentation

```

## Technologies Used

- **.NET 8**: Latest .NET framework
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Database (can be easily switched)
- **ASP.NET Core Web API**: REST API framework
- **Dependency Injection**: Built-in .NET DI container

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd BookStoreApp
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Update database connection string**
   - Open `appsettings.json` in `BookStoreApp.API`
   - Update the `DefaultConnection` string

4. **Run database migrations**
   ```bash
   dotnet ef database update --project BookStoreApp.Infrastructure --startup-project BookStoreApp.API
   ```

5. **Run the application**
   ```bash
   dotnet run --project BookStoreApp.API
   ```

6. **Access the API**
   - API: `https://localhost:7001` or `http://localhost:5001`

## API Endpoints (Phase 1)

### Books
- `GET /api/books` - Get all books
- `GET /api/books/{id}` - Get book by ID
- `POST /api/books` - Create a new book
- `PUT /api/books/{id}` - Update an existing book
- `DELETE /api/books/{id}` - Delete a book

## Business Rules (Phase 1)

### Book Entity Rules
- Title is required and must not exceed 200 characters
- Author is required and must not exceed 100 characters
- ISBN must be unique and follow ISBN-13 format
- Price must be greater than 0
- Publication date cannot be in the future

## Learning Objectives

This phase teaches:
1. **Clean Architecture principles** and layer separation
2. **Entity Framework Core** setup and configuration
3. **Dependency Injection** in .NET applications
4. **RESTful API** design principles
5. **Domain modeling** and entity design
6. **Service layer** implementation

## Next Phase Preview

In Phase 2, we'll add:
- FluentValidation for robust input validation
- Custom validation rules and error messages
- Validation middleware for centralized validation handling

## Contributing

This project is designed for learning purposes. Each phase builds upon the previous one, demonstrating progressive enhancement of the application architecture.

## 🌟 Branch Strategy & Phase Implementation

Each phase is implemented in a separate branch, building upon the previous phase. This allows you to see the evolution of the application architecture step by step.

### 📋 Branch Structure & Implementation Guides

| Phase | Branch Name | Description | Implementation Guide Location |
|-------|-------------|-------------|------------------------------|
| **Phase 1** | `main` | Basic Clean Architecture Setup | `docs/Phase_01_Implementation_Guide.md` (in `main` branch) |
| **Phase 2** | `phase_02_validation-fluent` | FluentValidation Integration | `docs/Phase_02_Implementation_Guide.md` (in `phase_02_validation-fluent` branch) |
| **Phase 3** | `phase_03_cqrs-mediatr` | CQRS Pattern with MediatR | `docs/Phase_03_Implementation_Guide.md` (in `phase_03_cqrs-mediatr` branch) |
| **Phase 4** | `phase_04_repository-pattern` | Repository & Unit of Work | `docs/Phase_04_Implementation_Guide.md` (in `phase_04_repository-pattern` branch) |
| **Phase 5** | `phase_05_error-handling` | Global Error Handling & Logging | `docs/Phase_05_Implementation_Guide.md` (in `phase_05_error-handling` branch) |
| **Phase 6** | `phase_06_authentication-jwt` | JWT Authentication & Authorization | `docs/Phase_06_Implementation_Guide.md` (in `phase_06_authentication-jwt` branch) |
| **Phase 7** | `phase_07_swagger-documentation` | API Documentation with Swagger | `docs/Phase_07_Implementation_Guide.md` (in `phase_07_swagger-documentation` branch) |
| **Phase 8** | `phase_08_testing` | Comprehensive Testing | `docs/Phase_08_Implementation_Guide.md` (in `phase_08_testing` branch) |

### 🚀 How to Work with Branches

1. **Start with Phase 1** (Current branch: `main`)
   ```bash
   git checkout main
   # Follow: docs/Phase_01_Implementation_Guide.md
   ```

2. **Move to Next Phase**
   ```bash
   # Example: Moving to Phase 2
   git checkout phase_02_validation-fluent
   # Follow: docs/Phase_02_Implementation_Guide.md (available in this branch)
   ```

3. **Compare Phases**
   ```bash
   # See what changed between phases
   git diff main..phase_02_validation-fluent
   ```

### 📚 Branch-Specific Documentation

Each branch contains:
- **Complete working code** for that phase
- **Phase-specific implementation guide** in `docs/` folder
- **Updated README** with current phase information
- **All previous functionality** plus new features

> **Important**: Implementation guides are only available in their respective branches. This ensures each branch is self-contained and shows exactly what needs to be implemented for that specific phase.

### 🎯 Quick Phase Navigation

- **Current Branch**: `main` (Phase 1)
- **Current Implementation Guide**: `docs/Phase_01_Implementation_Guide.md`
- **To implement Phase 2**: Switch to `phase_02_validation-fluent` branch
- **To implement Phase 3**: Switch to `phase_03_cqrs-mediatr` branch
- *(Continue pattern for subsequent phases)*

---

**Author**: Clean Architecture Learning Series  
**Version**: 1.0 (Phase 1)  
**Last Updated**: May 28, 2025
