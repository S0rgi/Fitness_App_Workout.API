namespace Fitness_App_Workout.API.Models;
public class ChallengeExercise
{
    public Guid Id { get; set; }
    public Guid ChallengeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public float Weight { get; set; } // кг

}
