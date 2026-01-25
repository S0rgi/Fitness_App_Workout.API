namespace Fitness_App_Workout.API.Dto;


public class CreateExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public List<SetDto> Sets { get; set; }
}

public class SetDto
{
    public int Reps { get; set; }
    public float Weight { get; set; }
}