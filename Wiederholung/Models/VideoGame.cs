
using Microsoft.EntityFrameworkCore;

namespace Models;
/// <summary>
/// A Class representing a VideoGame. 
/// This is for a private video game collection and reviewing.
/// </summary>
public class VideoGame
{
    public int ID { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Developer { get; set; } = String.Empty;
    public string ReleaseDate { get; set; } = String.Empty;
    public int Rating { get; set; }
    public string Review { get; set; } = String.Empty;
}
// Create new VideoGame DTO inside a record without id
public record NewVideoGameDTO(string Title, string Developer, string ReleaseDate, int Rating, string Review);


public class VideoGameContext : DbContext
{
    public VideoGameContext(DbContextOptions<VideoGameContext> options) : base(options) { }

    public DbSet<VideoGame> VideoGames => Set<VideoGame>();
}


// public class DbContext
// {
//     public DbContext(DbContextOptions<T> options)
//     {
//         
//     }
// }