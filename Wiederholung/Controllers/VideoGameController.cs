namespace Models.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;
[ApiController]
[Route("[controller]")]
public class VideoGameController : ControllerBase
{
    private readonly VideoGameContext _context;
    public VideoGameController(VideoGameContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Route("/")]
    public ActionResult<List<VideoGame>> GetAll()
    {
        var videoGames = _context.VideoGames.ToList();
        return Ok(videoGames);
    }

    [HttpGet]
    [Route("/{id}")]
    public ActionResult<VideoGame> GetByID(int id)
    {
        var videoGame = _context.VideoGames.Find(id);

        if(videoGame == null) NotFound();
        return Ok(videoGame);
    }

    [HttpPost]
    [Route("/")]
    public ActionResult<VideoGame> CreateOne([FromBody] NewVideoGameDTO newVideoGameDTO)
    {
        var videoGame = new VideoGame
        {
            Title = newVideoGameDTO.Title,
            Developer = newVideoGameDTO.Developer,
            ReleaseDate = newVideoGameDTO.ReleaseDate,
            Rating = newVideoGameDTO.Rating,
            Review = newVideoGameDTO.Review
        };

        _context.VideoGames.Add(videoGame);
        _context.SaveChanges();

        return Created($"/{videoGame.ID}", videoGame);
    }
}