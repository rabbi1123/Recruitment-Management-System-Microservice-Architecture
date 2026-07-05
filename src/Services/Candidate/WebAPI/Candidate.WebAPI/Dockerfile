#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_AUTH_TOKEN
ARG GITHUB_ACTOR=docker
WORKDIR /src
COPY ["NuGet.config", "./"]
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS="{\"endpointCredentials\": [{\"endpoint\":\"https://nuget.pkg.github.com/rabbi1123/index.json\", \"username\":\"${GITHUB_ACTOR}\", \"password\":\"${NUGET_AUTH_TOKEN}\"}]}"
COPY ["Services/Candidate/WebAPI/Candidate.WebAPI/Candidate.WebAPI.csproj", "Services/Candidate/WebAPI/Candidate.WebAPI/"]
COPY ["Services/Candidate/Core/Candidate.Application/Candidate.Application.csproj", "Services/Candidate/Core/Candidate.Application/"]
COPY ["Services/Candidate/Core/Candidate.Domain/Candidate.Domain.csproj", "Services/Candidate/Core/Candidate.Domain/"]
COPY ["Services/Candidate/Infrastructure/Candidate.Infrastructure/Candidate.Infrastructure.csproj", "Services/Candidate/Infrastructure/Candidate.Infrastructure/"]
RUN dotnet restore "./Services/Candidate/WebAPI/Candidate.WebAPI/Candidate.WebAPI.csproj"
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