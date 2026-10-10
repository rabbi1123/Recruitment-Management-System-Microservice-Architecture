FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Notification/WebAPI/Notification.WebAPI/Notification.WebAPI.csproj", "Services/Notification/WebAPI/Notification.WebAPI/"]
COPY ["Services/Notification/Core/Notification.Application/Notification.Application.csproj", "Services/Notification/Core/Notification.Application/"]
COPY ["Services/Notification/Core/Notification.Domain/Notification.Domain.csproj", "Services/Notification/Core/Notification.Domain/"]
COPY ["Services/Notification/Infrastructure/Notification.Infrastructure/Notification.Infrastructure.csproj", "Services/Notification/Infrastructure/Notification.Infrastructure/"]
COPY ["BuildingBlocks/Messaging.Contracts/Messaging.Contracts.csproj", "BuildingBlocks/Messaging.Contracts/"]
COPY ["BuildingBlocks/Messaging.RabbitMq/Messaging.RabbitMq.csproj", "BuildingBlocks/Messaging.RabbitMq/"]
RUN dotnet restore "./Services/Notification/WebAPI/Notification.WebAPI/Notification.WebAPI.csproj"
COPY . .
WORKDIR "/src/Services/Notification/WebAPI/Notification.WebAPI"
RUN dotnet build "./Notification.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notification.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.WebAPI.dll"]
