using Fitness_App_Workout.API.Dto;
using Fitness_App_Workout.API.Grpc;
using Fitness_App_Workout.API.Interfaces;
using Fitness_App_Workout.API.Models;
using Fitness_App_Workout.API.Data;
using Microsoft.EntityFrameworkCore;

namespace Fitness_App_Workout.API.Service;

public class WorkoutService : IWorkoutService
{
    private readonly WorkoutDbContext _dbContext;
        public WorkoutService(WorkoutDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<CreateWorkoutResult> CreateWorkout(CreateWorkoutRequest request, UserResponse user)
    {
        var workout = new Workout
        {
            UserId = Guid.Parse(user.Id),
            Title = request.Title,
            Date = request.dateTime?? DateTime.UtcNow,
            Exercises = request.Exercises.Select(e => new WorkoutExercise
            {
                Name = e.Name,
                Sets = e.Sets.Select(s => new WorkoutSet
                {
                    Reps = s.Reps,
                    Weight = s.Weight,
                    IsOwnWeight = s.IsOwnWeight ?? false
                }).ToList()

            }).ToList()
        };

        _dbContext.Workouts.Add(workout);
        await _dbContext.SaveChangesAsync();

        return new CreateWorkoutResult(true,null,workout.Id);
    }

    public async  Task<DeleteWorkoutResult> DeleteWorkout(string workoutId, UserResponse user)
    {
         if (!Guid.TryParse(workoutId, out var id))
            return new DeleteWorkoutResult(false,"Invalid WorkoutId format");

        var workout = await _dbContext.Workouts.FirstOrDefaultAsync(w => w.Id == id);
        if (workout == null)
        {
            new DeleteWorkoutResult(false,"Workout not found");
        }

        _dbContext.Workouts.Remove(workout);
        await _dbContext.SaveChangesAsync();
        
        return new DeleteWorkoutResult(true,null);;
    }

    public async Task<WorkoutProfileResult> GetProfile(UserResponse user)
    {
        var userId = Guid.Parse(user.Id);

        var query = _dbContext.Workouts
            .Where(w => w.UserId == userId)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var last30Days = DateTime.UtcNow.AddDays(-30);

        var last30Count = await query
            .Where(w => w.Date >= last30Days)
            .CountAsync();
        var res = new WorkoutProfileDto
        {
            WorkoutsTotal = totalCount,
            WorkoutsLast30Days = last30Count
        };
        return new WorkoutProfileResult(true, null,res );
    }


    public async Task<GetWorkoutResult> GetWorkout(string workoutId, UserResponse user)
    {
         if (!Guid.TryParse(workoutId, out var id))
            return new GetWorkoutResult(false,"Invalid WorkoutId format",null);

        var workout = await _dbContext.Workouts
            .AsNoTracking()
            .Include(w => w.Exercises)
                .ThenInclude(e => e.Sets)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workout == null)
            return new GetWorkoutResult(false, "Workout not found", null);

        var dto = new CreateWorkoutDto
        {
            Date = workout.Date,
            Id = workout.Id,
            Exercises = workout.Exercises.Select(e => new WorkoutExerciseDto
            {
                Name = e.Name,
                Sets = e.Sets.Select(s => new WorkoutSetDto
                {
                    Reps = s.Reps,
                    Weight = s.Weight,
                    IsOwnWeight = s.IsOwnWeight
                }).ToList()
            }).ToList()
        };

        return new GetWorkoutResult(true, null, dto);
    }

    public async Task<GetWorkoutListResult> GetWorkoutList(UserResponse user, WorkoutFilter filter)
    {
        var query = _dbContext.Workouts
            .Where(w => w.UserId == Guid.Parse(user.Id))
            .AsQueryable();

        if (filter.From.HasValue)
            query = query.Where(w => w.Date >=  DateTime.SpecifyKind(filter.From.Value, DateTimeKind.Local).ToUniversalTime());

        if (filter.To.HasValue)
            query = query.Where(w => w.Date <=  DateTime.SpecifyKind(filter.To.Value, DateTimeKind.Local).ToUniversalTime());

        query = query.OrderByDescending(w => w.Date);

        if (filter.Last.HasValue)
            query = query.Take(filter.Last.Value);

        var workouts = await query.ToListAsync();

        return new GetWorkoutListResult(true, null, workouts);
    }
}