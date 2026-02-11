# Chartify

A modern .NET application for tracking and serving Spotify global charts data with real-time caching and automated daily ingestion.

## Features

- 📊 **Real-time Chart Data** - Fetch Spotify's global top 100 charts
- ⚡ **Redis Caching** - 24-hour cache TTL for optimal performance
- 🔄 **Automated Ingestion** - Daily scheduled data download via Quartz.NET (1 AM UTC)
- 🎭 **Clean Architecture** - Domain-Driven Design with separated concerns
- 🐳 **Containerized** - Docker & Docker Compose for easy deployment
- ☁️ **Cloud Database** - Supabase PostgreSQL for reliable data storage
- ✅ **Comprehensive Tests** - xUnit with Moq for unit testing
- 📝 **Structured Logging** - Serilog for detailed application insights
- 🔐 **Environment-based Config** - Secure credential management via .env

## Tech Stack

- **Runtime**: .NET 9.0
- **Database**: Supabase (PostgreSQL 16)
- **Cache**: Redis 7
- **ORM**: Entity Framework Core 9
- **Scheduler**: Quartz.NET
- **Testing**: xUnit, Moq
- **Logging**: Serilog
- **Web Scraping**: Playwright
- **Containerization**: Docker & Docker Compose

## Architecture

```
Chartify/
├── src/
│   ├── Chartify.Domain/           # Entities (Chart, Track)
│   ├── Chartify.Application/      # Business Logic (ChartService)
│   ├── Chartify.Infrastructure/   # Data Access (Repository, Cache, DbContext)
│   ├── Chartify.Api/              # REST API (Controllers, Health checks)
│   ├── Chartify.Ingestion/        # Data Pipeline (Download, Parse, Import)
│   ├── Chartify.*.Tests/          # Unit Tests (xUnit)
│   └── Chartify.sln
└── docker-compose.yml             # Multi-container orchestration
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose (optional, for containerized deployment)
- Git

### Installation

```bash
# Clone the repository
git clone https://github.com/yourusername/chartify.git
cd chartify

# Restore dependencies
dotnet restore

# Create .env file with your Supabase credentials
cp .env.example .env
# Edit .env with your connection strings
```

### Running Locally

```bash
# Apply database migrations
dotnet ef database update --project src/Chartify.Infrastructure

# Run the API
dotnet run --project src/Chartify.Api/Chartify.Api.csproj
# API available at http://localhost:8080

# Run ingestion (manual trigger)
dotnet run --project src/Chartify.Ingestion/Chartify.Ingestion.csproj

# Run tests
dotnet test
```

### Running with Docker

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

## API Endpoints

### Get Global Top 100 Chart
```bash
GET http://localhost:8080/api/charts
```

**Response:**
```json
{
  "country": "global",
  "date": "2026-02-10",
  "tracks": [
    {
      "id": "1",
      "name": "Top Song",
      "artist": "Top Artist",
      "rank": 1,
      "streams": 10000000
    }
  ]
}
```

### Health Check
```bash
GET http://localhost:8080/health
```

## Database Schema

### Charts Table
```sql
CREATE TABLE "Charts" (
  "Id" SERIAL PRIMARY KEY,
  "Country" VARCHAR(50) NOT NULL,
  "Date" DATE NOT NULL,
  "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### ChartEntries Table
```sql
CREATE TABLE "ChartEntries" (
  "Id" SERIAL PRIMARY KEY,
  "ChartId" INT NOT NULL REFERENCES "Charts"(Id),
  "TrackName" VARCHAR(255) NOT NULL,
  "Artist" VARCHAR(255) NOT NULL,
  "Rank" INT NOT NULL,
  "Streams" BIGINT,
  "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## Scheduled Ingestion

The ingestion pipeline runs automatically **daily at 1 AM UTC** via Quartz.NET:

1. **Download** - Playwright scrapes Spotify Charts CSV
2. **Parse** - In-memory CSV parsing and validation
3. **Import** - Batch insert to Supabase PostgreSQL
4. **Cache Invalidation** - Redis cache refreshed on new data

Monitor ingestion logs:
```bash
docker logs chartify-ingestion -f
```

## Configuration

Create a `.env` file in the root directory:

```bash
ASPNETCORE_ENVIRONMENT=Production

# Supabase PostgreSQL (Session Pooler)
ConnectionStrings__Postgres=postgresql://postgres:PASSWORD@aws-1-eu-central-1.pooler.supabase.com:5432/postgres?sslmode=require

# Redis Cache
ConnectionStrings__Redis=redis:6379,abortConnect=false,connectTimeout=5000,connectRetry=3
```

## Testing

Run all unit tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test src/Chartify.Api.Tests/
dotnet test src/Chartify.Application.Tests/
dotnet test src/Chartify.Domain.Tests/
```

Test coverage includes:
- ✅ ChartService caching and database queries
- ✅ ChartsController error handling and responses
- ✅ Chart entity creation and validation

## Deployment

### Docker Compose (Local/VPS)
```bash
docker-compose up -d --build
```

### Cloud Deployment (Railway/Render/AWS)
1. Push code to GitHub
2. Connect repository to cloud platform
3. Set environment variables in platform UI
4. Deploy - service runs 24/7

Recommended platforms:
- **Railway** - $5/month, easiest setup
- **Render** - Free tier available
- **AWS ECS** - Most scalable option

## Development

### Project Structure

- **Domain Layer** - Pure domain models (Chart, Track)
- **Application Layer** - Business logic and interfaces
- **Infrastructure Layer** - Data access, caching, repositories
- **API Layer** - HTTP endpoints and controllers
- **Ingestion Layer** - Scheduled data pipeline

### Adding a New Feature

1. Define entity in `Chartify.Domain`
2. Create interface in `Chartify.Application`
3. Implement in `Chartify.Infrastructure`
4. Expose via `Chartify.Api` controller
5. Add tests in respective `.Tests` project

## Troubleshooting

### Database Connection Error
```
"Keine solche Datei oder Verzeichnis"
```
**Solution**: Check `.env` has correct Supabase credentials and hostname is reachable

### Redis Connection Failed
**Solution**: Ensure Redis is running in Docker or locally on port 6379

### Ingestion Job Not Running
**Solution**: Check service logs - `docker logs chartify-ingestion`

## Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'feat: add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions, please open a GitHub issue or contact the maintainer.

---

**Built with ❤️ using .NET 9 & clean architecture principles**