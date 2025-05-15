using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Fitness_App_Workout.API.Grpc;
using Grpc.Core;

public class GrpcAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedObjectResult("Missing token");
            return;
        }

        var grpcClient = httpContext.RequestServices.GetService<UserService.UserServiceClient>();

        if (grpcClient == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        try
        {
            var user = await grpcClient.ValidateTokenAsync(new TokenRequest { AccessToken = token });
            httpContext.Items["User"] = user;
        }
        catch (RpcException ex)
        {
            context.Result = new UnauthorizedObjectResult("Invalid token: " + ex.Status.Detail);
            return;
        }

        await next();
    }
}
