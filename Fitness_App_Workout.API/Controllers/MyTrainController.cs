using Microsoft.AspNetCore.Mvc;
using Fitness_App_Workout.API.Data;
using Fitness_App_Workout.API.Models;
using Fitness_App_Workout.API.Grpc;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Workout.API.Dto;
using Fitness_App_Workout.API.Interfaces;
using System.Threading.Tasks;
using Fitness_App_Workout.Service;
using Microsoft.AspNetCore.Authorization;
[ApiController]
[Route("api/[controller]")]
public class MyTrainController : ControllerBase
{
    private readonly IWorkoutService _workoutService;
    private readonly Helper _helper;

    public MyTrainController(IWorkoutService workoutService, Helper helper)
    {
        _workoutService = workoutService;
        _helper = helper;
    }

    [AllowAnonymous]
    [HttpGet("my_last_train")]
    [ProducesResponseType(typeof(WorkoutProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMyLastTrain([FromQuery] WorkoutFilter filter)
    {
        var me = new UserResponse
        {
            Id = "0197208d-b102-742b-8927-fcc7266114db",
            Email = "",
            Username = ""
        };
        var res = await _workoutService.GetWorkoutList(me,filter);
        if (res.result){
            return Ok(res.workouts.Select(x => _workoutService.GetWorkout(x.Id.ToString(),me).Result.workout));
        }
         return Problem(title: "Get my last workout failed", detail: res.ErrorMessage, statusCode: StatusCodes.Status400BadRequest);
    }
}
