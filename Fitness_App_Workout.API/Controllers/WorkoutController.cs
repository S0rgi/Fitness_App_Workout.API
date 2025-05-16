using Microsoft.AspNetCore.Mvc;
using Fitness_App_Workout.API.Data;
using Fitness_App_Workout.API.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Fitness_App_Workout.API.Grpc;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Workout.API.Dto;
[ApiController]
[Route("api/[controller]")]
[GrpcAuthorize]
public class WorkoutController : ControllerBase
{
    private readonly WorkoutDbContext _dbContext;

    public WorkoutController(WorkoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpPost]
    public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutRequest request)
    {
        var user = HttpContext.Items["User"] as UserResponse;

        var workout = new Workout
        {
            UserId = Guid.Parse(user.Id),
            Title = request.Title,
            Date = DateTime.UtcNow,
            Exercises = request.Exercises.Select(e => new WorkoutExercise
            {
                Name = e.Name,
                Sets = e.Sets,
                Reps = e.Reps,
                Weight =e.Weight
                
            }).ToList()
        };

        _dbContext.Workouts.Add(workout);
        await _dbContext.SaveChangesAsync();

        return Ok(new { workout.Id });
    }

    [HttpGet]
    public IActionResult GetWorkoutList()
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var workouts = _dbContext.Workouts
            .Where(w => w.UserId == Guid.Parse(user.Id))
            .ToList();

        return Ok(workouts);
    }

    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetWorkout(string workoutId)
    {
        var user = HttpContext.Items["User"] as UserResponse;

        if (!Guid.TryParse(workoutId, out var id))
            return BadRequest("Invalid WorkoutId format");

        var workout = await _dbContext.Workouts
            .Include(w => w.Exercises)  // Загрузка связанных упражнений
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
            return NotFound("Workout not found");

        return Ok(workout);
    }
        [HttpDelete("{workoutId}")]
    public async Task<IActionResult> DeleteWorkout(string workoutId)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        if (!Guid.TryParse(workoutId, out var id))
            return BadRequest("Invalid WorkoutId format");

        var workout = await _dbContext.Workouts.FirstOrDefaultAsync(w => w.Id == id);
        if (workout == null)
        {
            return NotFound("Workout not found");
        }

        _dbContext.Workouts.Remove(workout);
        await _dbContext.SaveChangesAsync();
        
        return Ok();
    }
}
