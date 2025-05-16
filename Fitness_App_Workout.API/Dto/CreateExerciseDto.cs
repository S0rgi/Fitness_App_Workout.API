namespace Fitness_App_Workout.API.Dto;


public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public int Weight { get; set; }
}
