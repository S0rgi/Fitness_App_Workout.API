using Fitness_App_Workout.API.Models;
namespace Fitness_App_Workout.API.Dto;

public class ChallengeAnswerDto
{
    public ChallengeStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
}
