# MarketDataPipeline — Dummy Market Data Replay Engine

A demo project that simulates a market data pipeline and replay engine. The backend reads dummy market data, performs calculations, persists results, and publishes them to a frontend dashboard that controls and visualises replays.

This repository contains a .NET backend (API + replay engine), an infrastructure layer for data providers and persistence, and a simple frontend dashboard.

---

## Key Features

- **Replay Engine:** Reads time-series CSV market data and replays it at adjustable speed.
- **Calculations:** Produces derived tick calculations and publishes them for downstream consumers.
- **Persistence:** Stores calculated ticks in Postgres.
- **Realtime UI:** A frontend dashboard connects via SignalR to control replay and show results.
- **Dockerized:** Compose file to run the full stack locally.

---

## Repository Layout

- `src/MarketReplay.Api/` — API and replay worker, SignalR hubs, background services.
- `src/MarketReplay.Core/` — Domain models, services and application logic.
- `src/MarketReplay.Infrastructure/` — Data providers (CSV), persistence, Kafka helpers, and Postgres helpers.
- `src/MarketReplay.Tests/` — Unit tests.
- `frontend/` — Minimal frontend dashboard used to control and view replays.
- `scripts/postgres/init/` — Database initialization SQL.
- `data/` — Example CSV market data used by the CSV provider.
- `docker-compose.yml` — Development compose to run services together.

---

## Prerequisites

- .NET SDK (version matching the solution; e.g. .NET 8 or newer). Install from https://dotnet.microsoft.com/
- Docker & Docker Compose (for running full stack via containers).
- (Optional) Node.js/npm if you want to extend the frontend development workflow.

---

## Quickstart — Docker Compose (recommended)

Run the whole system (API, DB, frontend) with Docker Compose. From the repository root:

First, make sure Docker Desktop is open on your machine.

PowerShell
```
docker-compose up --build
```

This builds images and starts the services. The API will be reachable on ports defined in `docker-compose.yml` (Swagger page currently accessible via http://localhost:5000/swagger/index.html). The frontend is served from the `frontend` service or static file server configured in the compose file (currently accessible via http://localhost:8080)

To stop:

PowerShell
```
docker-compose down
```

---

## Quickstart — Local development (without Docker)

1. Start Postgres. You can run Postgres locally or use Docker only for the DB. If you use Docker for DB:

PowerShell
```
docker run --name marketdata-postgres -e POSTGRES_PASSWORD=password -e POSTGRES_USER=admin -e POSTGRES_DB=marketreplay -p 5432:5432 -d postgres:15
```

2. Apply the SQL init scripts in `scripts/postgres/init/` to create required tables.

3. From the repository root, build and run the API:

PowerShell
```
cd src/MarketReplay.Api
dotnet build
dotnet run --project MarketReplay.Api.csproj
```

4. Open the frontend: either run the `frontend` service from Compose or open `frontend/index.html` in a browser.

Notes:
- App configuration is in `src/MarketReplay.Api/appsettings.Development.json` and `appsettings.json`. Update connection strings and settings as needed.
- The CSV provider reads files from the `data/` folder by default. See `src/MarketReplay.Infrastructure/Data/DataDirectory.cs` and `CsvMarketDataProvider.cs` to change locations.

---

## Running Tests

Run unit tests from the solution root:

PowerShell
```
dotnet test src/MarketReplay.Tests/MarketReplay.Tests.csproj
```

---

## Architecture Overview

- **API / Replay Worker (`MarketReplay.Api`)**: Hosts SignalR endpoints for the frontend to control the replay and streams state updates. Contains background workers (`ReplayWorker.cs`) that drive the replay loop.
- **Core (`MarketReplay.Core`)**: Contains domain models (replays, ticks), services, and application-level logic.
- **Infrastructure (`MarketReplay.Infrastructure`)**: Provides implementations for data access — CSV provider, Postgres persistence, Kafka producers/consumers, and helper utilities.
- **Frontend (`frontend/`)**: Minimal dashboard that connects to the API via SignalR and provides controls for starting/stopping replays and viewing calculated ticks.

SignalR is used for low-latency updates from the API to the UI. The pipeline is intentionally lightweight to make it easy to extend or replace parts (CSV -> Kafka -> Postgres, etc.).

---

## Design notes

- **Decoupling with Kafka:** The pipeline uses Kafka to decouple ingestion/processing from downstream work. The replay engine publishes calculated ticks to Kafka topics; SignalR delivery and persistence are implemented as independent Kafka consumers. This keeps the data ingestion and calculation hot path lightweight and low-latency because broadcasting and storing results happens asynchronously off the topic.
- **Authentication (JWT):** The project uses JWT-based authentication for the frontend and API. Obtain a JWT token using the `/login` endpoint, then include the token in API requests using an `Authorization: Bearer <token>` header. For SignalR connections, the client should supply the token as the `access_token` query parameter when connecting (or via the transport-specific header if supported) so the hub can validate the caller.
- **Using Swagger with auth:** Swagger UI is available at `/swagger/index.html`. To call protected endpoints from Swagger:
	- First call the `POST /login` endpoint to retrieve a JWT token.
	- In Swagger click the **Authorize** button and paste the token (NOT including the `Bearer ` prefix).
	- After authorising, you can call protected API endpoints from Swagger.


