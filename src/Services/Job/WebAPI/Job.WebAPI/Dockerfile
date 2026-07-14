#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Job/WebAPI/Job.WebAPI/Job.WebAPI.csproj", "Services/Job/WebAPI/Job.WebAPI/"]
COPY ["Services/Job/Core/Job.Application/Job.Application.csproj", "Services/Job/Core/Job.Application/"]
COPY ["Services/Job/Core/Job.Domain/Job.Domain.csproj", "Services/Job/Core/Job.Domain/"]
COPY ["Services/Job/Infrastructure/Job.Infrastructure/Job.Infrastructure.csproj", "Services/Job/Infrastructure/Job.Infrastructure/"]
RUN dotnet restore "./Services/Job/WebAPI/Job.WebAPI/Job.WebAPI.csproj"
COPY . .
WORKDIR "/src/Services/Job/WebAPI/Job.WebAPI"
RUN dotnet build "./Job.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Job.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Job.WebAPI.dll"]