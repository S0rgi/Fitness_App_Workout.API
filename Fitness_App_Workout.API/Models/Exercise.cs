namespace Fitness_App_Workout.API.Models
{
    public class WorkoutExercise
    {
        public Guid Id { get; set; }
        public Guid WorkoutId { get; set; }
        public string Name { get; set; } // Например, "Жим лежа"
        public List<WorkoutSet> Sets { get; set; } = new();
    }

    public class WorkoutSet
    {
        public Guid Id { get; set; }
        public Guid WorkoutExerciseId { get; set; }

        public int Reps { get; set; }
        public float Weight { get; set; }

        public WorkoutExercise WorkoutExercise { get; set; }
    }
}
