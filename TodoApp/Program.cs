using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Rewrite;

namespace TodoApp;

public record Todo(int Id, string Name, DateTime DueDate, bool IsCompleted);

public class Program()
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<ITaskService>(new InMemoryTaskService());

        // Builds the web app
        var app = builder.Build();

        app.Use(
            async (context, next) =>
            {
                Console.WriteLine(
                    $"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}"
                );
                // needed to run the route handling logic which itself is a middleware, the request will stop otherwise
                await next.Invoke();
                Console.WriteLine(
                    $"[{context.Request.Method} {context.Request.Path} {DateTime.UtcNow}"
                );
            }
        );

        app.UseRewriter(new RewriteOptions().AddRedirect("tasks/(.*)", "todos/$1"));

        app.MapGet("todos/", (ITaskService service) => TypedResults.Ok(service.GetTodos()));

        // Gets the id as a route parameter
        app.MapGet(
            "todos/{id}",
            // Return type of the route handler is Results (i.e. it'll either be T1 (Ok), or T2 (NotFound))
            Results<Ok<Todo>, NotFound> (int id, ITaskService service) =>
            {
                var resultTodo = service.GetTodoById(id);
                return resultTodo is null ? TypedResults.NotFound() : TypedResults.Ok(resultTodo);
            }
        );

        // Run this when a client sends a POST request to "/todos"
        app.MapPost(
                "/todos",
                // Our Minimal API takes this complex type and deserializes it to a type that you can interact with
                (Todo task, ITaskService service) =>
                {
                    Todo todo = service.AddTodo(task);

                    // Status code 201 is sent with Created
                    return TypedResults.Created($"/todos/{todo.Id}", todo);
                }
            )
            .AddEndpointFilter(
                async (context, next) =>
                {
                    // get arg
                    var taskArg = context.GetArgument<Todo>(0);

                    // initiate errors
                    var errors = new Dictionary<string, string[]>();

                    if (taskArg.IsCompleted)
                    {
                        // nameof ensures if IsCompleted property of Todo record is renamed with IDE refactoring tools, the expression automatically updates
                        errors.Add(nameof(Todo.IsCompleted), ["Cannot add completed Todo"]);
                    }

                    var today = DateTime.UtcNow.Date; // Use UTC and date-only comparison

                    if (taskArg.DueDate.Date < today)
                    {
                        errors.Add(
                            nameof(Todo.DueDate),
                            [
                                $"Cannot add a past date. Date Entered: {taskArg.DueDate}. Date today: {today}",
                            ]
                        );
                    }

                    if (errors.Count > 0)
                    {
                        return Results.ValidationProblem(errors);
                    }

                    return await next(context);
                }
            );

        // DELETE
        app.MapDelete(
            "/todos/{id}",
            Results<NoContent, NotFound> (int id, ITaskService service) =>
            {
                service.DeleteTodoById(id);
                return TypedResults.NoContent();
            }
        );

        app.Run();
    }
}

interface ITaskService
{
    Todo? GetTodoById(int id);

    List<Todo> GetTodos();

    void DeleteTodoById(int id);

    Todo AddTodo(Todo todo);
}

public class InMemoryTaskService : ITaskService
{
    private readonly List<Todo> _todos = [];
    private readonly object _lock = new object();
    private int _nextId = 1;

    public Todo? GetTodoById(int id)
    {
        lock (_lock)
        {
            return _todos.FirstOrDefault(t => t.Id == id);
        }
    }

    public List<Todo> GetTodos()
    {
        lock (_lock)
        {
            return new List<Todo>(_todos); // Return a copy to avoid external modification
        }
    }

    public void DeleteTodoById(int id)
    {
        lock (_lock)
        {
            _todos.RemoveAll(todo => todo.Id == id);
        }
    }

    public Todo AddTodo(Todo todo)
    {
        lock (_lock)
        {
            var newTodo = todo with { Id = _nextId++ }; // Assign a new ID
            _todos.Add(newTodo);
            return newTodo;
        }
    }
}
