using Microsoft.AspNetCore.Mvc;
using Fitness_App_Workout.API.Data;
using Fitness_App_Workout.API.Models;
using Fitness_App_Workout.API.Dto;
using Microsoft.EntityFrameworkCore;
using Fitness_App_Workout.API.Grpc;
using Fitness_App_Workout.API.Service;
using System.Runtime.CompilerServices;
using Grpc.Core;
using System.Text.Json;
[ApiController]
[Route("api/[controller]")]
[GrpcAuthorize]
public class ChallengesController : ControllerBase
{
    private readonly WorkoutDbContext _context;
    private readonly UserService.UserServiceClient _grpcClient;
    private readonly MessagePublisher _publisher;

    public ChallengesController(
        WorkoutDbContext context,
        UserService.UserServiceClient grpcClient,
        MessagePublisher publisher)
    {
        _context = context;
        _grpcClient = grpcClient;
        _publisher = publisher;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChallenge([FromBody] CreateChallengeRequest request)
    {
        var user = HttpContext.Items["User"] as UserResponse;
        if (user.Username == request.FriendName)
            return BadRequest("Инициатор и получатель не могут совпадать.");

        var friendshipRequest = new FriendshipRequest
        {
            UserId = user.Id,
            FriendName = request.FriendName
        };

        FriendshipResponse friendshipResponse;
        try
        {
            friendshipResponse = await _grpcClient.CheckFriendshipAsync(friendshipRequest);
        }
        catch (RpcException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка проверки дружбы: {ex.Status.Detail}");
        }

        var challenge = new Challenge
        {
            Id = Guid.NewGuid(),
            InitiatorId = Guid.Parse(user.Id),
            RecipientId = Guid.Parse(friendshipResponse.FriendId),
            Duration = request.Duration,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow,
            Status = ChallengeStatus.Pending,
            Exercises = request.Exercises.Select(e => new ChallengeExercise
            {
                Id = Guid.NewGuid(),
                Name = e.Name,
                Sets = e.Sets,
                Reps = e.Reps,
                Weight = e.Weight
            }).ToList()
        };

        _context.Challenges.Add(challenge);
        await _context.SaveChangesAsync();

        var notification = new NotificationMessage
        {
            Type = "challenge_invite",
            SenderName = user.Username,
            RecipientEmail = friendshipResponse.Email
        };

        await _publisher.PublishAsync(JsonSerializer.Serialize(notification));

        return CreatedAtAction(nameof(GetChallengeById), new { id = challenge.Id }, challenge);
    }

[HttpPost("{id}/respond")]
public async Task<IActionResult> RespondToChallenge(Guid id, [FromBody] ChallengeAnswerDto request)
{
    var user = HttpContext.Items["User"] as UserResponse;
    if (user == null)
        return Unauthorized();

    var challenge = await _context.Challenges.FindAsync(id);
    if (challenge == null)
        return NotFound();

    if (challenge.RecipientId != Guid.Parse(user.Id))
        return Forbid("You are not the recipient of this challenge.");

    if (challenge.Status != ChallengeStatus.Pending)
        return BadRequest("Challenge is already answered.");

    var allowed = new[] { ChallengeStatus.Accepted, ChallengeStatus.Rejected, ChallengeStatus.Completed, ChallengeStatus.Failed };
    if (!allowed.Contains(request.Status))
        return BadRequest("Invalid challenge status response.");

    challenge.Status = request.Status;

    if (request.Status == ChallengeStatus.Completed)
        challenge.CompletedAt = request.CompletedAt ?? DateTime.UtcNow;

    await _context.SaveChangesAsync();

    // 👇 Получаем инициатора через gRPC
    UserResponse sender;
    try
    {
        sender = await _grpcClient.GetUserByIdAsync(new UserRequest { Id = challenge.InitiatorId.ToString() });
    }
    catch (RpcException ex)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка получения отправителя: {ex.Status.Detail}");
    }

    var notification = new NotificationMessage
    {
        Type = "challenge_response",
        SenderName = user.Username,
        RecipientEmail = sender.Email,
        Action = request.Status.ToString()
    };

    await _publisher.PublishAsync(JsonSerializer.Serialize(notification));

    return Ok(challenge);
}


    [HttpGet("{id}")]
    public async Task<IActionResult> GetChallengeById(Guid id)
    {
        var challenge = await _context.Challenges
            .Include(c => c.Exercises)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challenge == null)
            return NotFound();

        return Ok(challenge);
    }
    [HttpGet("my")]
    public async Task<IActionResult> GetMyChallenges()
    {
        var user = HttpContext.Items["User"] as UserResponse;
        if (user == null)
            return Unauthorized();

        var userId =Guid.Parse( user.Id);

        var challenges = await _context.Challenges
            .Include(c => c.Exercises)
            .Where(c => c.InitiatorId == userId || c.RecipientId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return Ok(challenges);
    }
}
