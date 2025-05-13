using Microsoft.EntityFrameworkCore;
using Fitness_App_Workout.API.Data;
using Grpc.Net.Client;
//using WorkoutService.Grpc;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using DotNetEnv;
using Fitness_App_Workout.API.Grpc;
var builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

// Загрузка переменных из EnvironmentVariables в Configuration
builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration["ConnectionStrings:WorkoutDb"]
                      ?? throw new InvalidOperationException("Connection string not found.");

// Регистрация DbContext
builder.Services.AddDbContext<WorkoutDbContext>(options =>
    options.UseNpgsql(connectionString));

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
var authGrpcAddress = Environment.GetEnvironmentVariable("AUTH_GRPC_ADDRESS")
                    ?? "https://localhost:5001";

builder.Services.AddSingleton(_ => new AuthGrpc.AuthGrpcClient(GrpcChannel.ForAddress(authGrpcAddress)));

// Контроллеры / API
builder.Services.AddControllers();
// Поддержка кастомного порта (для Fly)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8081";
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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
