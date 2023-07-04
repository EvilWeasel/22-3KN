var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/joke", () => { throw new NotImplementedException(); });
app.MapGet("/joke/{id}", (int id) => { throw new NotImplementedException(); });
app.MapPost("/joke", () => { throw new NotImplementedException(); });
app.MapDelete("/joke/{id}", (int id) => { throw new NotImplementedException(); });
app.MapPut("/joke/{id}", (int id) => { throw new NotImplementedException(); });


app.Run();


class Joke
{
    public int ID { get; set; } // Id of the Joke => Server-Generated
	public string Setup { get; set; } = String.Empty; // Setup for the Joke
    public string Punchline { get; set; } = "";  // The funny bit
	public int FunLevel { get; set; } // Integer describing the funnyness of the Joke (0..5)
}