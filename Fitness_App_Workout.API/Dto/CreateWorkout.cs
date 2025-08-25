namespace Fitness_App_Workout.API.Dto;

public class CreateWorkoutRequest
{
    public string Title { get; set; } = string.Empty;
    public List<CreateExerciseDto> Exercises { get; set; } = new();
}

public record CreateWorkoutResponse( Guid workoutId );