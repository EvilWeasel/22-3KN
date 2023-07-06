using System.Collections.Concurrent;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Configuration => Dependency-Injection
builder.Services.AddEndpointsApiExplorer(); // => API-Dokumentation WebUI
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() {
        Title = "DevJokes-Unlimited",
        Version = "v1",
        Description = "A basic CRUD-API, which can save and server Developer-Jokes...",
        Contact = new() {
            Name = "Tobias Weltner",
            Email = "tw@lg.com",
            Url = new("https://www.lg.com")
        },
    });
    options.IncludeXmlComments("""C:\Users\Tobia\source\Unterricht\KN22-3\JokesAPI\bin\Debug\net7.0\JokesAPI.xml""");
}); // => Generator
builder.Services.AddDbContext<JokeContext>(options => 
{
    options.UseSqlServer("""Server=localhost;User=sa;Password=Diamond!_Yellow_Slug;Database=Jokes;TrustServerCertificate=True""");
});

#region WasSindStrings
var string1 = "blabla{asfdsjhk}\ndlkfjsld"; // String
// var string2 = $"blabla{userInput}\ndlkfjsld"; // Interpolated-String
var string3 = """

blabla{asfdsjhk}\ndlkfjsld



"""; // Sting-Literal
// Das ist literally nur ein String!
var string4 = @"blabla{asfdsjhk}\ndlkfjsld"; // String-Literal
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Legacy Code - Don't use!
var jokeRepo = new ConcurrentDictionary<int, Joke>();
var nextJokeId = 0;

// GET All Jokes
app.MapGet("/joke", (JokeContext context) => context.Joke.ToList()); // jokeRepo.Values
// GET One Joke by ID
app.MapGet("/joke/{id}", (int id, JokeContext context) => 
{
    var joke = context.Joke.Find(id);
    if (joke != null)
    {
        return Results.Ok(joke);
    }
    return Results.NotFound();
})
    .WithName("GetJokeByID")
    .Produces<Joke>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithOpenApi(options => {
        options.Parameters[0].Description = "The Filter for which Joke to return";
        return options;
    });
// POST Create one new Joke
app.MapPost("/joke", (CreateJokeDto newJoke, JokeContext context) => 
{
    // Create a new ID for the joke
    // var newId = Interlocked.Increment(ref nextJokeId); // Threadsafe Variante von nextJokeId++;
    // Convert DTO into Joke class
    var jokeToAdd = new Joke
    {
        Setup = newJoke.Setup,
        Punchline = newJoke.Punchline,
        FunLevel = newJoke.FunLevel
    };
    // Add Joke to DB
    // Return http-status and joke
    var joke = context.Joke.Add(jokeToAdd);
    context.SaveChanges();
    if (joke == null)
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    return Results.Created($"/joke/{jokeToAdd.ID}", jokeToAdd);
})
    .Produces<Joke>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status500InternalServerError)
    .WithOpenApi(options => {
        options.OperationId = "CreateJoke";
        options.Description = "Creates a new Joke";
        options.RequestBody = new OpenApiRequestBody {
            Content = {
                ["application/json"] = new OpenApiMediaType {
                    Schema = new OpenApiSchema {
                        Reference = new OpenApiReference {
                            Id = "CreateJokeDto",
                            Type = ReferenceType.Schema
                        }
                    }
                }
            }
        };
        options.Responses[StatusCodes.Status201Created.ToString()].Description = "Joke was created";
        options.Responses[StatusCodes.Status500InternalServerError.ToString()].Description = "Something went wrong";
        return options;
    });
app.MapPut("/joke/{id}", (int id, CreateJokeDto updatedJoke, JokeContext context) => 
{
    var jokeToChange = context.Joke.Find(id);
    // Get current value from dictionary
    if (jokeToChange == null)
        return Results.NotFound();
    // Change joke
    jokeToChange.Setup = updatedJoke.Setup;
    jokeToChange.Punchline = updatedJoke.Punchline;
    jokeToChange.FunLevel = updatedJoke.FunLevel;

    context.SaveChanges();
    // Send response with joke
    return Results.Ok(jokeToChange);
});
app.MapDelete("/joke/{id}", (int id, JokeContext context) => 
{
    var jokeToRemove = context.Joke.Find(id);
    if (jokeToRemove == null)
        return Results.NotFound();
    context.Remove(jokeToRemove);
    context.SaveChanges();
    return Results.Ok(jokeToRemove);
});

app.Run();