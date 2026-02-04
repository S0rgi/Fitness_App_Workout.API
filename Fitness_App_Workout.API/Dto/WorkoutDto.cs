public class WorkoutDto
{
    public Guid Id { get; set; }
    public List<WorkoutExerciseDto> Exercises { get; set; }
}

public class WorkoutExerciseDto
{
    public string Name { get; set; }
    public List<WorkoutSetDto> Sets { get; set; }
}

public class WorkoutSetDto
{
    public int Reps { get; set; }
    public float Weight { get; set; }
}
