using Microsoft.EntityFrameworkCore;
using Fitness_App_Workout.API.Data;
using Fitness_App_Workout.API.Service;
using Grpc.Net.Client;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using DotNetEnv;
using Fitness_App_Workout.API.Grpc;
using System.Net.Security;
using System.Net;
using Grpc.Net.Client.Web;
using System.Net.Http;
using Fitness_App_Workout.API.Interfaces;
var builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

// Загрузка переменных из EnvironmentVariables в Configuration
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionStrings:WorkoutDb"]
                      ?? throw new InvalidOperationException("Connection string not found.");

// Регистрация DbContext
builder.Services.AddDbContext<WorkoutDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IWorkoutService, WorkoutService>();
// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Workout API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите JWT токен",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer" }
            },
            new string[] {} 
        }
    });
});

// gRPC-клиент для AuthService (адрес можно задать через ENV)
var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());

var channel = GrpcChannel.ForAddress(builder.Configuration.GetConnectionString("Grpc_Server"), new GrpcChannelOptions
{
    HttpHandler = handler,
    UnsafeUseInsecureChannelCallCredentials = true
});

builder.Services.AddSingleton(_ => new UserService.UserServiceClient(channel));
builder.Services.AddSingleton<MessagePublisher>(sp =>
{
    var uriRabbitmq = builder.Configuration.GetConnectionString("RabbitMq");
    var pingUrl = builder.Configuration.GetConnectionString("PingNotifyUrl");
                Console.WriteLine(pingUrl);
    return new MessagePublisher(uriRabbitmq, pingUrl);
});
// Контроллеры / API

builder.Services.AddControllers();

// Поддержка кастомного порта (для Fly)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    serverOptions.ListenAnyIP(int.Parse(port));
});
// Запуск приложения
var app = builder.Build();

// Применение миграций автоматически при запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkoutDbContext>();
    db.Database.Migrate();
}

// Swagger и middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
