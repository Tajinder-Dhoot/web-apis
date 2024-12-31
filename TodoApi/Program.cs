using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddEndpointsApiExplorer(); // Adds support for API endpoint discovery.
builder.Services.AddSwaggerGen(); // Configures the app to generate Swagger documentation for the API.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); //Enables Swagger middleware for generating API documentation.
    app.UseSwaggerUI(); // Provides a web-based UI for testing the API endpoints.
}

app.UseHttpsRedirection(); // Redirects all HTTP requests to HTTPS for security.

app.UseAuthorization(); // Adds middleware to handle authorization. This checks if a user has the proper permissions to access an endpoint.

app.MapControllers(); //Maps the controller actions (e.g., GET, POST, PUT, DELETE) to specific API endpoints.

app.Run(); // Starts the application and begins listening for HTTP requests.
