using System.Collections.Concurrent;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);
// Configuration
builder.Services.AddEndpointsApiExplorer(); // => API-Dokumentation WebUI
builder.Services.AddSwaggerGen(); // => Generator

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

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
}).Produces<Joke>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);
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


class Joke
{
    public int ID { get; set; } // Id of the Joke => Server-Generated
	public string Setup { get; set; } = String.Empty; // Setup for the Joke
    public string Punchline { get; set; } = "";  // The funny bit
	public int FunLevel { get; set; } // Integer describing the funnyness of the Joke (0..5)
}

// DTO => Data-Transfer-Object
// Abbild eines Datenmodells, das für die Übertragung über HTTP verändert wird
record CreateJokeDto(string Setup, string Punchline, int FunLevel);

/*
    {
        "Setup": "Wie viele SQL Programmierer braucht man um eine Glühbirne zu tauschen?",
        "Punchline": "ca. 65-90",
        "FunLevel": 5
    }
*/