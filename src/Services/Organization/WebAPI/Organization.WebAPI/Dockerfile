#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Organization/WebAPI/Organization.WebAPI/Organization.WebAPI.csproj", "Services/Organization/WebAPI/Organization.WebAPI/"]
COPY ["Services/Organization/Core/Organization.Application/Organization.Application.csproj", "Services/Organization/Core/Organization.Application/"]
COPY ["Services/Organization/Core/Organization.Domain/Organization.Domain.csproj", "Services/Organization/Core/Organization.Domain/"]
COPY ["Services/Organization/Infrastructure/Organization.Infrastructure/Organization.Infrastructure.csproj", "Services/Organization/Infrastructure/Organization.Infrastructure/"]
RUN dotnet restore "./Services/Organization/WebAPI/Organization.WebAPI/Organization.WebAPI.csproj"
COPY . .
WORKDIR "/src/Services/Organization/WebAPI/Organization.WebAPI"
RUN dotnet build "./Organization.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Organization.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Organization.WebAPI.dll"]