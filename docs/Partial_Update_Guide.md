# Partial Update (PATCH) Examples

## How to Use Partial Updates in BookStore API

The BookStore API now supports partial updates using HTTP PATCH method. This allows you to update only specific fields of a book without sending the entire book data.

## Examples

### 1. Update Only Price
```json
PATCH /api/books/{book-id}
Content-Type: application/json

{
    "price": 29.99
}
```

### 2. Update Title and Author
```json
PATCH /api/books/{book-id}
Content-Type: application/json

{
    "title": "Clean Code - Second Edition",
    "author": "Robert C. Martin"
}
```

### 3. Update Multiple Fields
```json
PATCH /api/books/{book-id}
Content-Type: application/json

{
    "price": 49.99,
    "description": "Updated description with new content",
    "pageCount": 500,
    "publisher": "New Publisher"
}
```

### 4. Update Publication Date
```json
PATCH /api/books/{book-id}
Content-Type: application/json

{
    "publicationDate": "2024-01-15T00:00:00Z"
}
```

## Key Benefits

1. **Efficiency**: Only send the data you want to update
2. **Bandwidth**: Smaller payloads, especially important for mobile apps
3. **Flexibility**: Update any combination of fields
4. **Safety**: Business rules are still enforced for updated fields

## HTTP Status Codes

- `200 OK`: Update successful, returns updated book
- `400 Bad Request`: Invalid data or empty patch
- `404 Not Found`: Book with specified ID doesn't exist
- `409 Conflict`: Business rule violation
- `500 Internal Server Error`: Server error

## Using with Postman or HTTP Client

1. Set method to `PATCH`
2. Set URL to `https://localhost:{port}/api/books/{book-id}`
3. Set Content-Type header to `application/json`
4. Add request body with only the fields you want to update
5. Send request

## Comparison: PUT vs PATCH

### PUT (Full Update)
- Replaces entire resource
- Requires all fields
- Overwrites missing fields with defaults

### PATCH (Partial Update)  
- Updates only specified fields
- Ignores fields not provided
- Preserves existing values for unspecified fields
