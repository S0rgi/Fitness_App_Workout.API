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
public class WorkoutController : ControllerBase
{
    private readonly WorkoutDbContext _dbContext;
    private readonly UserService.UserServiceClient _authClient;

    public WorkoutController(WorkoutDbContext dbContext, UserService.UserServiceClient authClient)
    {
        _dbContext = dbContext;
        _authClient = authClient;
    }


    [HttpPost]
    public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutRequest request)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Missing token");

        UserResponse user;
        try
        {
            user = await _authClient.ValidateTokenAsync(new TokenRequest { AccessToken = token });
        }
        catch (RpcException ex)
        {
            return Unauthorized("Invalid token: " + ex.Status.Detail);
        }

        var workout = new Workout
        {
            UserId = Guid.Parse(user.Id),
            Title = request.Title,
            Date = DateTime.UtcNow,
            Exercises = request.Exercises.Select(e => new Exercise
            {
                Name = e.Name,
                Sets = e.Sets,
                Reps = e.Repetitions
            }).ToList()
        };

        _dbContext.Workouts.Add(workout);
        await _dbContext.SaveChangesAsync();

        return Ok(new { workout.Id });
    }
    [HttpGet]
    public async Task<IActionResult> GetWorkoutList(){
         var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Missing token");

        UserResponse user;
        try
        {
            user = await _authClient.ValidateTokenAsync(new TokenRequest { AccessToken = token });
        }
        catch (RpcException ex)
        {
            return Unauthorized("Invalid token: " + ex.Status.Detail);
        }
        return Ok(_dbContext.Workouts.Where(Workout => Workout.UserId ==Guid.Parse(user.Id ) ).ToList());
    }
    [HttpGet("{workoutId}")]
    public async Task<IActionResult> GetWorkout(string workoutId)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Missing token");
        UserResponse user;
        try
        {
            user = await _authClient.ValidateTokenAsync(new TokenRequest { AccessToken = token });
        }
        catch (RpcException ex)
        {
            return Unauthorized("Invalid token: " + ex.Status.Detail);
        }

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
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Missing token");
        UserResponse user;
        try
        {
            user = await _authClient.ValidateTokenAsync(new TokenRequest { AccessToken = token });
        }
        catch (RpcException ex)
        {
            return Unauthorized("Invalid token: " + ex.Status.Detail);
        }

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
