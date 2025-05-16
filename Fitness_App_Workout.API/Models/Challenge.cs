namespace Fitness_App_Workout.API.Models;
public enum ChallengeStatus
{
    Pending,
    Accepted,
    Completed,
    Failed,
    Rejected
}

public class Challenge
{
    public Guid Id { get; set; } // Уникальный ID

    public Guid InitiatorId { get; set; } // Пользователь, отправивший вызов
    public Guid RecipientId { get; set; } // Пользователь, которому отправлен вызов

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public TimeSpan? Duration { get; set; } // Или DateTime Deadline { get; set; }

    public string? Message { get; set; } // Комментарий

    public ChallengeStatus Status { get; set; } = ChallengeStatus.Pending;

    // Навигационные свойства
    public List<ChallengeExercise> Exercises { get; set; } = new();
}
