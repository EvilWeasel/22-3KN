// C# XML Inline Documentation
using Microsoft.EntityFrameworkCore;
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

class JokeContext : DbContext
{
    public JokeContext(DbContextOptions<JokeContext> options) : base(options) {}

    public DbSet<Joke> Joke => Set<Joke>();
    // public DbSet<Joke> BestOfJokes => Set<Joke>();
    // public DbSet<Customer> Customer => Set<Customer>();
}