using Microsoft.AspNetCore.Mvc;
using Fitness_App_Workout.API.Data;
using Fitness_App_Workout.API.Models;
using Fitness_App_Workout.API.Grpc;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Workout.API.Dto;
using Fitness_App_Workout.API.Interfaces;
using System.Threading.Tasks;
using Fitness_App_Workout.Service;
[ApiController]
[Route("api/[controller]")]
[GrpcAuthorize]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutService _workoutService;
    private readonly Helper _helper;

    public WorkoutController(IWorkoutService workoutService, Helper helper)
    {
        _workoutService = workoutService;
        _helper = helper;
    }


    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateWorkoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateWorkout([FromBody] CreateWorkoutRequest request)
    {
        var user = HttpContext.Items["User"] as UserResponse;

        var res = await _workoutService.CreateWorkout(request, user);
        if (res.result){
            return Ok(new CreateWorkoutResponse(res.WorkoutId));
        }
         return Problem(title: "Create workout failed", detail: res.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("workouList")]
    [ProducesResponseType(typeof(List<Workout>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWorkoutList()
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var res = await _workoutService.GetWorkoutList(user);
                if (res.result)
            return Ok(res.workouts);

         return Problem(title: "Get workouts list failed", detail: res.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("workouList/{friendsname}")]
    [ProducesResponseType(typeof(List<Workout>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWorkoutListFriends(string friendsname)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var friendshipCheck = await _helper.CheckFriendshipAsync(user, friendsname);
        if (!friendshipCheck.result)
        {
                     return Problem(title: "Get workouts list failed", detail: friendshipCheck.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);

        }
        var friend = new UserResponse
        {
            Id = friendshipCheck.friend.FriendId,
            Email = friendshipCheck.friend.Email,
            Username = friendsname
        };
        var res = await _workoutService.GetWorkoutList(friend);
                if (res.result)
            return Ok(res.workouts);

         return Problem(title: "Get workouts list failed", detail: res.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
    }

    [HttpGet("{workoutId}")]
    [ProducesResponseType(typeof(Workout), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetWorkout(string workoutId)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var res = await _workoutService.GetWorkout(workoutId, user);
        if (res.result)
            return Ok(res.workout);

        return Problem(title: "Get workout failed", detail: res.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);

    }
    [HttpDelete("{workoutId}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteWorkout(string workoutId)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        var res = await _workoutService.DeleteWorkout(workoutId, user);
        if (res.result)
            return Ok();

        return Problem(title: "Delete workout failed", detail: res.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
    }
}
