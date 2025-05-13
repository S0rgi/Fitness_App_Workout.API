namespace Fitness_App_Workout.API.Models
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public Guid WorkoutId { get; set; }
        public string Name { get; set; } // Например, "Жим лежа"
        public int Sets { get; set; }
        public int Reps { get; set; }
        public float Weight { get; set; } // кг
    }
}
