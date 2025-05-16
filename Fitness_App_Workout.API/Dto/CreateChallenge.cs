namespace Fitness_App_Workout.API.Dto;
public class CreateChallengeRequest
{
    public Guid InitiatorId { get; set; }
    public Guid RecipientId { get; set; }
    public TimeSpan? Duration { get; set; } // или DateTime Deadline
    public string? Message { get; set; }

    public List<CreateExerciseDto> Exercises { get; set; } = new();
}