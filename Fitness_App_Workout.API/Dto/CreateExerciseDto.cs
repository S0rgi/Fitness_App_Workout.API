namespace Fitness_App_Workout.API.Dto;
public class CreateWorkoutRequest
{
    public string Title { get; set; } = string.Empty;
    public List<CreateExerciseDto> Exercises { get; set; } = new();
}

public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Repetitions { get; set; }
}
