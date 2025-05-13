namespace Fitness_App_Workout.API.Models
{
    public class Workout
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // получаем через gRPC из токена
        public string Title { get; set; } // Например, "Грудь и спина"
        public DateTime Date { get; set; }
        public ICollection<Exercise> Exercises { get; set; }
    }
}
