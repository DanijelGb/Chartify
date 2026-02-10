# Chartify Docker Compose Configuration

This project is containerized with Docker and orchestrated using Docker Compose.

## Services

### PostgreSQL Database
- **Image**: postgres:16-alpine
- **Port**: 5432
- **Database**: chartify
- **Credentials**: postgres/postgres
- **Volume**: `postgres_data`

### Redis Cache
- **Image**: redis:7-alpine
- **Port**: 6379
- **Volume**: `redis_data`

### Chartify API
- **Build**: Multi-stage Docker build from root Dockerfile
- **Port**: 8080 (HTTP)
- **Dependencies**: PostgreSQL, Redis
- **Health Check**: Enabled

## Quick Start

### Prerequisites
- Docker
- Docker Compose

### Running the Project

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Remove volumes (clean reset)
docker-compose down -v
```

## Connection Details

### From API Container
- **PostgreSQL**: `Host=postgres;Port=5432;Database=chartify;Username=postgres;Password=postgres`
- **Redis**: `redis:6379`

### From Host Machine
- **PostgreSQL**: `localhost:5432`
- **Redis**: `localhost:6379`
- **API**: `http://localhost:8080`

## Environment Variables

The API automatically uses these environment variables from docker-compose.yml:
- `ASPNETCORE_ENVIRONMENT`: Set to `Production`
- `ConnectionStrings__Postgres`: Database connection string
- `ConnectionStrings__Redis`: Redis connection string

## Volumes

- **postgres_data**: Persists PostgreSQL data
- **redis_data**: Persists Redis data
- **./src/Chartify.Api/Logs**: Application logs (mounted from host)

## Network

All services are connected via `chartify-network` bridge network, enabling service-to-service communication by hostname.

## Health Checks

- **PostgreSQL**: Checks connection every 10s (max 5 retries)
- **Redis**: PING every 10s (max 5 retries)
- **API**: HTTP request to /health every 30s (starts after 40s, max 3 retries)

## Development vs Production

To switch environments, modify the `docker-compose.yml`:
- Change `ASPNETCORE_ENVIRONMENT` to `Development` for debug logging
- Update connection strings if using different credentials

## Troubleshooting

### Cannot connect to database
```bash
# Check if postgres is healthy
docker-compose ps
docker-compose logs postgres
```

### Redis connection refused
```bash
# Check redis container
docker-compose logs redis
docker exec chartify-redis redis-cli ping
```

### API not starting
```bash
# View API logs
docker-compose logs api
# Ensure database is ready before API starts
```
