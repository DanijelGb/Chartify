# Chartify - Daily Ingestion Setup

## Overview
The ingestion service is configured to run daily at **1 AM UTC** using Quartz.NET scheduler.

## Fixed Issues
- **Cron Expression Format**: Quartz.NET requires 6-field format (seconds minutes hours day-of-month month day-of-week)
  - Correct format: `0 0 1 * * ?` (1 AM UTC daily)
  - This was causing "Unexpected end of expression" errors

## Running the Service

### Option 1: Direct Execution (Testing)
```bash
cd C:\Users\DaciBaci\Desktop\Chartify
./ingestion-publish/Chartify.Ingestion.exe
```

### Option 2: With Environment Variables (Production)
```bash
cd C:\Users\DaciBaci\Desktop\Chartify
# Load .env and run
export $(cat .env | xargs) && ./ingestion-publish/Chartify.Ingestion.exe
```

### Option 3: Docker Container (Recommended for Production)
```bash
docker-compose up -d ingestion
```

## Building for Production

### Rebuild the ingestion service:
```bash
dotnet clean src/Chartify.Ingestion/Chartify.Ingestion.csproj
dotnet publish src/Chartify.Ingestion/Chartify.Ingestion.csproj -c Release -o ./ingestion-publish
```

## Deployment Options

### Windows Service (Requires Admin)
If you have admin privileges, you can create a Windows Service:
```powershell
New-Service -Name ChartifyIngestion `
  -BinaryPathName 'C:\Users\DaciBaci\Desktop\Chartify\ingestion-publish\Chartify.Ingestion.exe' `
  -DisplayName 'Chartify Daily Chart Ingestion' `
  -StartupType Automatic

Start-Service -Name ChartifyIngestion
```

### Windows Scheduled Task (Requires Admin)
Run the setup script with admin privileges:
```powershell
powershell -ExecutionPolicy Bypass -File "C:\Users\DaciBaci\Desktop\Chartify\setup-scheduled-task.ps1"
```

### Docker Deployment
The ingestion service is available as a Docker container. Update `docker-compose.yml` to include:
```yaml
ingestion:
  build:
    context: .
    dockerfile: Dockerfile.Ingestion
  env_file: .env
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
  depends_on:
    postgres:
      condition: service_healthy
```

## Configuration

### .env File
Located at: `C:\Users\DaciBaci\Desktop\Chartify\.env`

Required variables:
- `ASPNETCORE_ENVIRONMENT` - Set to `Production`
- `ConnectionStrings__Postgres` - Supabase PostgreSQL connection string
- `ConnectionStrings__Redis` - Redis cache connection string

### Scheduled Time
- **Time**: 1 AM UTC daily
- **Cron Expression**: `0 0 1 * * ?` (Quartz 6-field format)
- **File**: `src/Chartify.Ingestion/Program.cs` (line 47)

## Logs
Logs are written to: `src/Chartify.Ingestion/Logs/chartify-*.log`

Rolling logs are retained for 14 days with daily rotation.

## Monitoring
After deployment, verify the job runs by:
1. Check logs daily at 1:15 AM UTC
2. Query the database to confirm new chart data appears
3. Monitor the ingestion-publish folder for any runtime errors

## Troubleshooting

### Issue: Service won't start
- Verify the executable exists: `Test-Path 'C:\Users\DaciBaci\Desktop\Chartify\ingestion-publish\Chartify.Ingestion.exe'`
- Check Event Viewer Application logs for detailed error
- Try running the exe directly to see actual error message

### Issue: Cron expression errors
- Ensure you're using 6-field format: `0 0 1 * * ?`
- Rebuild and republish after changing the cron expression

### Issue: Database connection errors
- Verify .env file is loaded with correct connection strings
- Test database connectivity before deploying
- Check that Supabase pooler endpoint is accessible

### Issue: Missing dependencies
- Run: `dotnet restore src/Chartify.Ingestion/Chartify.Ingestion.csproj`
- Rebuild: `dotnet publish src/Chartify.Ingestion/Chartify.Ingestion.csproj -c Release -o ./ingestion-publish`
