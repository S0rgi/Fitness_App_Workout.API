# Базовый образ (ASP.NET 9.0)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем CSPROJ и восстанавливаем зависимости
COPY Fitness_App_Workout.API/Fitness_App_Workout.API.csproj Fitness_App_Workout.API/
RUN dotnet restore "Fitness_App_Workout.API/Fitness_App_Workout.API.csproj"

# Копируем всё остальное
COPY . .

# Сборка проекта
WORKDIR /src/Fitness_App_Workout.API
RUN dotnet build "Fitness_App_Workout.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этап публикации
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fitness_App_Workout.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап (рантайм)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fitness_App_Workout.API.dll"]
