namespace Fitness_App_Workout.API.Dto;
public class CreateChallengeRequest
{
    public string FriendName { get; set; }
    public TimeSpan? Duration { get; set; } // или DateTime Deadline
    public string? Message { get; set; }

    public List<CreateChallengExerciseDto> Exercises { get; set; } = new();
}
public class CreateChallengExerciseDto
{
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public int Weight { get; set; }
}