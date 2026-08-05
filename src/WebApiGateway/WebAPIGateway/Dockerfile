FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApiGateway/WebAPIGateway/WebAPIGateway.csproj", "WebApiGateway/WebAPIGateway/"]
RUN dotnet restore "./WebApiGateway/WebAPIGateway/WebAPIGateway.csproj"
COPY WebApiGateway/WebAPIGateway/ WebApiGateway/WebAPIGateway/
WORKDIR "/src/WebApiGateway/WebAPIGateway"
RUN dotnet build "./WebAPIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAPIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPIGateway.dll"]
