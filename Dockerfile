# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["Chartify.sln", "./"]
COPY ["src/src.sln", "src/"]
COPY ["src/Chartify.Api/Chartify.Api.csproj", "src/Chartify.Api/"]
COPY ["src/Chartify.Application/Chartify.Application.csproj", "src/Chartify.Application/"]
COPY ["src/Chartify.Domain/Chartify.Domain.csproj", "src/Chartify.Domain/"]
COPY ["src/Chartify.Infrastructure/Chartify.Infrastructure.csproj", "src/Chartify.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Chartify.Api/Chartify.Api.csproj"

# Copy source code
COPY ["src/", "src/"]

# Build the application
WORKDIR "/src/src/Chartify.Api"
RUN dotnet build "Chartify.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Chartify.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/Logs

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Chartify.Api.dll"]
