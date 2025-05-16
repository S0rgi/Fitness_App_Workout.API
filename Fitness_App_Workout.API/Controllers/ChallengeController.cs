using Microsoft.AspNetCore.Mvc;
using Fitness_App_Workout.API.Data;
using Fitness_App_Workout.API.Models;
using Fitness_App_Workout.API.Dto;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("api/[controller]")]
[GrpcAuthorize]
public class ChallengesController : ControllerBase
{
    private readonly WorkoutDbContext _context;

    public ChallengesController(WorkoutDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChallenge([FromBody] CreateChallengeRequest request)
    {
        if (request.InitiatorId == request.RecipientId)
            return BadRequest("Инициатор и получатель не могут совпадать.");

        var challenge = new Challenge
        {
            Id = Guid.NewGuid(),
            InitiatorId = request.InitiatorId,
            RecipientId = request.RecipientId,
            Duration = request.Duration,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow,
            Status = ChallengeStatus.Pending,
            Exercises = request.Exercises.Select(e => new ChallengeExercise
            {
                Id = Guid.NewGuid(),
                Name = e.Name,
                Sets = e.Sets,
                Reps = e.Reps,
                Weight = e.Weight
            }).ToList()
        };

        _context.Challenges.Add(challenge);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetChallengeById), new { id = challenge.Id }, challenge);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChallengeById(Guid id)
    {
        var challenge = await _context.Challenges
            .Include(c => c.Exercises)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challenge == null)
            return NotFound();

        return Ok(challenge);
    }
}
