FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Identity/Identity.WebAPI/Identity.WebAPI.csproj", "Identity/Identity.WebAPI/"]
COPY ["Identity/Identity.Application/Identity.Application.csproj", "Identity/Identity.Application/"]
COPY ["Identity/Identity.Domain/Identity.Domain.csproj", "Identity/Identity.Domain/"]
COPY ["Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Identity/Identity.Infrastructure/"]
RUN dotnet restore "./Identity/Identity.WebAPI/Identity.WebAPI.csproj"
COPY Identity/ Identity/
WORKDIR "/src/Identity/Identity.WebAPI"
RUN dotnet build "./Identity.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.WebAPI.dll"]
