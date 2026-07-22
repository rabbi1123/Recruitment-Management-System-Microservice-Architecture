#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Recruitment/WebAPI/Recruitment.WebAPI/Recruitment.WebAPI.csproj", "Services/Recruitment/WebAPI/Recruitment.WebAPI/"]
COPY ["Services/Recruitment/Core/Recruitment.Application/Recruitment.Application.csproj", "Services/Recruitment/Core/Recruitment.Application/"]
COPY ["Services/Recruitment/Core/Recruitment.Domain/Recruitment.Domain.csproj", "Services/Recruitment/Core/Recruitment.Domain/"]
COPY ["Services/Recruitment/Infrastructure/Recruitment.Infrastructure/Recruitment.Infrastructure.csproj", "Services/Recruitment/Infrastructure/Recruitment.Infrastructure/"]
RUN dotnet restore "./Services/Recruitment/WebAPI/Recruitment.WebAPI/Recruitment.WebAPI.csproj"
COPY . .
WORKDIR "/src/Services/Recruitment/WebAPI/Recruitment.WebAPI"
RUN dotnet build "./Recruitment.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Recruitment.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Recruitment.WebAPI.dll"]