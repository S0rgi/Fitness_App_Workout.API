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
    private readonly AuthGrpc.AuthGrpcClient _authClient;

    public WorkoutController(WorkoutDbContext dbContext, AuthGrpc.AuthGrpcClient authClient)
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

        TokenResponse user;
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
            UserId = Guid.Parse(user.UserId),
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

}
