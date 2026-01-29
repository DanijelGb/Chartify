My Chartify app.


### Automated Data Ingestion

Spotify charts are ingested daily using a scheduled GitHub Actions workflow.
The ingestion pipeline:
- Downloads CSV data via Playwright
- Parses and validates data in-memory
- Persists charts to Postgres
- Enforces uniqueness at the database level

The API serves cached, read-only data derived from the ingestion pipeline.
