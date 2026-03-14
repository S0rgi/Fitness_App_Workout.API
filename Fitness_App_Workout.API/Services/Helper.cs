using Fitness_App_Workout.API.Grpc;
using Grpc.Core;

namespace Fitness_App_Workout.Service;

public class Helper
{
    private readonly UserService.UserServiceClient _grpcClient;
        public Helper(UserService.UserServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }
    public async Task<CheckFriendship> CheckFriendshipAsync(UserResponse user, string FriendName)
    {
        var friendshipRequest = new FriendshipRequest
        {
            UserId = user.Id,
            FriendName = FriendName
        };

        FriendshipResponse friendshipResponse;
        try
        {
            friendshipResponse = await _grpcClient.CheckFriendshipAsync(friendshipRequest);
        }
        catch (RpcException ex)
        {
            return new CheckFriendship(false, $"Ошибка проверки дружбы: {ex.Status.Detail}",null);
        }
        return new CheckFriendship(true, null ,friendshipResponse );
    }
}

public record CheckFriendship(bool result, string ErrorMessage, FriendshipResponse friend);