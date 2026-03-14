namespace Fitness_App_Workout.API.Dto;

public record WorkoutProfileDto
{
    public int WorkoutsTotal {get ; set;}
    public int WorkoutsLast30Days {get ; set;}
}