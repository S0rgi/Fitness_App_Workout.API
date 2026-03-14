namespace Fitness_App_Workout.API.Dto;

public class WorkoutFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int? Last { get; set; }
}