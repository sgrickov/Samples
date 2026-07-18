# InfoTrack Solicitors Report

Automates searching https://www.solicitors.com for conveyancing solicitors across a configurable list of UK locations, scrapes the results page (hand-rolled HTML parsing — **no 3rd-party scraping/HTML libraries** are used, per the brief), and turns the raw listings into an insight-oriented report: national + per-location summaries, top-rated firms, accreditation rates, and detection of solicitors that are new since the previous search for a given location.

Built as a .NET Core Web API (`/backend`) + Vue 3/TypeScript SPA (`/frontend`).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+ LTS](https://nodejs.org/) (includes npm)
- **SQL Server Express LocalDB** — ships with Visual Studio, or install standalone via the [SQL Server Express installer](https://www.microsoft.com/sql-server/sql-server-downloads) (select the "LocalDB" component). The instance name used is the default `MSSQLLocalDB`.

No other accounts, API keys, or external services are required.

## Running the backend

```powershell
cd backend
dotnet restore
dotnet run --project src/InfoTrack.Solicitors.Api --launch-profile http
```

The API starts on **http://localhost:5264**. On first run it automatically creates the `InfoTrackSolicitors` database on your LocalDB instance and applies all EF Core migrations (including seeding the 8 default locations) — there is no manual database setup step.

**Connection string**: `backend/src/InfoTrack.Solicitors.Api/appsettings.Development.json`, key `ConnectionStrings:DefaultConnection`. Default value:

```
Server=(localdb)\mssqllocaldb;Database=InfoTrackSolicitors;Trusted_Connection=True;TrustServerCertificate=True
```

Change this if you want to point at a different SQL Server/LocalDB instance.

## Running the frontend

```powershell
cd frontend
npm install
npm run dev
```

Open **http://localhost:5173**. The frontend's API base URL is set in `frontend/.env.development` (`VITE_API_BASE_URL=http://localhost:5264`) — update it if you run the backend on a different port.

CORS is already configured on the backend (`Program.cs`) to explicitly allow `http://localhost:5173`, so no extra setup is needed for the two to talk to each other locally.

## Database schema script

`db/schema.sql` contains the full idempotent schema-creation + seed-data script (generated from the EF Core migrations via `dotnet ef migrations script --idempotent`), covering the schema-creation deliverable. You do **not** need to run it manually — the backend applies migrations automatically on startup — but it's included for review, and is safe to run repeatedly against the same database.

## Using the app

1. **Locations** panel: edit the list of locations to search (add/remove/reset to the 8 defaults), then **Save**.
2. **Run Search**: submits the real conveyancing search form on solicitors.com for each saved location and scrapes the results (takes a few seconds per location).
3. The **report** below shows national and per-location summaries, nationally top-rated firms, a "new since last search" panel, and a full sortable results table per location (click a column header to sort).
4. **History**: browse past search runs, or delete individual runs / clear all history.

## Running the tests

```powershell
cd backend
dotnet test
```

19 tests covering: the hand-rolled HTML parser against a real captured results page (both listing-variant markups), the search orchestrator's persistence and new-solicitor detection (against EF Core InMemory), and the report builder's aggregation logic.

## Project structure

```
/backend
  src/InfoTrack.Solicitors.Api/     ASP.NET Core Web API — controllers, DI, CORS, appsettings
  src/InfoTrack.Solicitors.Core/    Scraping client, hand-rolled HTML parser, search orchestrator, report builder
  src/InfoTrack.Solicitors.Data/    EF Core entities, DbContext, migrations
  tests/InfoTrack.Solicitors.Tests/ xUnit tests
/frontend                          Vue 3 + TypeScript + Vite SPA
/db/schema.sql                     Generated DB schema + seed data script
```

## Notes

- The parser is hand-rolled (generic balanced-tag HTML extraction + focused field extractors) rather than using HtmlAgilityPack/AngleSharp/etc., per the assessment brief.
- Scraping depends on the live structure of solicitors.com at the time of writing; if the site's markup changes, the parser may need updating.
- "New since last search" is tracked using the firm's profile-page URL as a natural key; the very first search for a given location doesn't flag anything as new (nothing to compare against yet).
