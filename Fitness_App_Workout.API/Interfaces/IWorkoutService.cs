using Fitness_App_Workout.API.Dto;
using Fitness_App_Workout.API.Grpc;
using Fitness_App_Workout.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fitness_App_Workout.API.Interfaces;

public interface IWorkoutService
{
    public Task<CreateWorkoutResult> CreateWorkout(CreateWorkoutRequest request, UserResponse user);
    public Task<GetWorkoutListResult> GetWorkoutList(UserResponse user);
    public Task<GetWorkoutResult> GetWorkout(string workoutId,UserResponse user);
    public Task<DeleteWorkoutResult> DeleteWorkout(string workoutId,UserResponse user);


}
public record CreateWorkoutResult(bool result, string ErrorMessage,Guid WorkoutId);
public record GetWorkoutListResult(bool result, string ErrorMessage,List<Workout> workouts);
public record GetWorkoutResult(bool result, string ErrorMessage, WorkoutDto workout);
public record DeleteWorkoutResult(bool result, string ErrorMessage);