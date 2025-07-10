// Minimal Hosting Model

// builder provides APIs for configuring the application host
var builder = WebApplication.CreateBuilder(args);

// app configures the request response pipeline
// allows users to configure route handlers
var app = builder.Build();

// 3 components for defining how a server (eg. http://localhost:5161) would handle a request
// 1. HTTP Method: GET, POST, PUT, DELETE
// 2. URL Pattern: /, /products, /products/{id}
// 3. Handler: Function that processes the request and generates a response
app.MapGet("/", () => "Hello World!!");

// Runs HTTP server and initiate HTTP request processing pipeline
app.Run();
