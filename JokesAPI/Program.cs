using System.Collections.Concurrent;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Configuration
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var jokeRepo = new ConcurrentDictionary<int, Joke>();
var nextJokeId = 0;

// GET All Jokes
app.MapGet("/joke", () => jokeRepo.Values);
// GET One Joke by ID
app.MapGet("/joke/{id}", (int id) => 
{
    if (jokeRepo.TryGetValue(id, out Joke? joke))
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
app.MapPost("/joke", (CreateJokeDto newJoke) => 
{
    // Create a new ID for the joke
    var newId = Interlocked.Increment(ref nextJokeId); // Threadsafe Variante von nextJokeId++;
    // Convert DTO into Joke class
    var jokeToAdd = new Joke
    {
        ID = newId,
        Setup = newJoke.Setup,
        Punchline = newJoke.Punchline,
        FunLevel = newJoke.FunLevel
    };
    // Add Joke to DB
    // Return http-status and joke
    if (!jokeRepo.TryAdd(newId, jokeToAdd))
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    return Results.Created($"/joke/{newId}", jokeToAdd);
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
app.MapPut("/joke/{id}", (int id, CreateJokeDto updatedJoke) => 
{
    // Get current value from dictionary
    if (!jokeRepo.TryGetValue(id, out Joke? joke))
        return Results.NotFound();
    // Change joke
    joke.Setup = updatedJoke.Setup;
    joke.Punchline = updatedJoke.Punchline;
    joke.FunLevel = updatedJoke.FunLevel;

    // Send response with joke
    return Results.Ok(joke);
});
app.MapDelete("/joke/{id}", (int id) => 
{
    if (!jokeRepo.Remove(id, out Joke? joke))
        return Results.NotFound();
    
    return Results.Ok(joke);
});

app.Run();

// C# XML Inline Documentation
/// <summary>
/// Represents a Joke-Object
/// </summary>
class Joke
{
    public int ID { get; set; } // Id of the Joke => Server-Generated
	public string Setup { get; set; } = String.Empty; // Setup for the Joke
    public string Punchline { get; set; } = "";  // The funny bit
	public int FunLevel { get; set; } // Integer describing the funnyness of the Joke (0..5)
}
/// <summary>
/// DTO => Data-Transfer-Object.
/// Abbild eines Datenmodells, das für die Übertragung über HTTP verändert wird
/// </summary>
record CreateJokeDto(string Setup, string Punchline, int FunLevel);

/*
    {
        "Setup": "Wie viele SQL Programmierer braucht man um eine Glühbirne zu tauschen?",
        "Punchline": "ca. 65-90",
        "FunLevel": 5
    }
*/