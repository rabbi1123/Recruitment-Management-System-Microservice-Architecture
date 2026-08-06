#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_ACTOR=rabbi1123
WORKDIR /src
COPY ["NuGet.config", "./"]
COPY ["Services/Candidate/WebAPI/Candidate.WebAPI/Candidate.WebAPI.csproj", "Services/Candidate/WebAPI/Candidate.WebAPI/"]
COPY ["Services/Candidate/Core/Candidate.Application/Candidate.Application.csproj", "Services/Candidate/Core/Candidate.Application/"]
COPY ["Services/Candidate/Core/Candidate.Domain/Candidate.Domain.csproj", "Services/Candidate/Core/Candidate.Domain/"]
COPY ["Services/Candidate/Infrastructure/Candidate.Infrastructure/Candidate.Infrastructure.csproj", "Services/Candidate/Infrastructure/Candidate.Infrastructure/"]
COPY ["BuildingBlocks/Messaging.Contracts/Messaging.Contracts.csproj", "BuildingBlocks/Messaging.Contracts/"]
RUN --mount=type=secret,id=nuget_auth_token \
    dotnet nuget remove source github --configfile NuGet.config 2>/dev/null || true && \
    dotnet nuget add source "https://nuget.pkg.github.com/rabbi1123/index.json" \
      --name github \
      --username "${GITHUB_ACTOR}" \
      --password "$(cat /run/secrets/nuget_auth_token)" \
      --store-password-in-clear-text \
      --configfile NuGet.config && \
    dotnet restore "./Services/Candidate/WebAPI/Candidate.WebAPI/Candidate.WebAPI.csproj"
COPY . .
WORKDIR "/src/Services/Candidate/WebAPI/Candidate.WebAPI"
RUN dotnet build "./Candidate.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Candidate.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Candidate.WebAPI.dll"]