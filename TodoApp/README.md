# TodoApp

A simple Todo API built with ASP.NET Core Minimal API.

## Project Structure

```text
TodoApp/
├── Models/
│   └── Todo.cs                    # Todo data model
├── Services/
│   ├── ITaskService.cs           # Service interface
│   └── InMemoryTaskService.cs    # In-memory implementation
├── Endpoints/
│   └── TodoEndpoints.cs          # API endpoint definitions
├── Middleware/
│   └── RequestLoggingMiddleware.cs # Custom logging middleware
├── Filters/
│   └── TodoValidationFilter.cs   # Validation filter for Todo creation
├── Properties/
│   └── launchSettings.json       # Launch configuration
├── Program.cs                     # Application entry point
├── TodoApp.csproj                # Project file
├── appsettings.json              # Application configuration
└── appsettings.Development.json  # Development configuration
```

## Architecture

This application follows a clean architecture pattern with clear separation of concerns:

- **Models**: Contains data transfer objects and entities
- **Services**: Business logic and data access layer
- **Endpoints**: API endpoint definitions organized by feature
- **Middleware**: Custom middleware components
- **Filters**: Endpoint filters for cross-cutting concerns like validation

## Features

- **CRUD Operations**: Create, Read, Update, Delete todos
- **Validation**: Automatic validation for todo creation
- **Logging**: Request/response logging middleware
- **URL Rewriting**: Redirects `/tasks/*` to `/todos/*`
- **Swagger Documentation**: API documentation (in development mode)

## API Endpoints

- `GET /todos` - Get all todos
- `GET /todos/{id}` - Get a specific todo by ID
- `POST /todos` - Create a new todo
- `DELETE /todos/{id}` - Delete a todo by ID

## Running the Application

```bash
dotnet run
```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

In development mode, Swagger UI will be available at `https://localhost:5001/swagger`.

## Sample Todo Object

```json
{
  "id": 1,
  "name": "Sample Todo",
  "dueDate": "2025-07-15T00:00:00Z",
  "isCompleted": false
}
```
