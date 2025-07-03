# TaxCalculator

A full-stack Tax Calculator application built with ASP.NET Core (Minimal API, EF Core, Serilog) and Angular 19 (standalone, signals).

## Features
- Calculate tax based on salary and tax bands
- Employee management (list, detail, update salary)
- In-memory caching for tax bands
- Error handling and logging
- Health checks
- Modern Angular UI with signals and standalone components

## Tech Stack
- **Backend:** .NET 9, ASP.NET Core Minimal API, Entity Framework Core (SQLite), Serilog
- **Frontend:** Angular 19, Signals, Standalone Components, Bootstrap 5
- **Testing:** xUnit, Angular TestBed, Jasmine, Karma (UI)

## Getting Started

## Folder Structure
- `TaxCalculator.Api/` - .NET API project
- `TaxCalculator.Tests/` - API unit tests
- `TaxCalculatorUi/` - Angular UI project

## Notes
- The API uses SQLite by default (see `employee.db`) - Connection string managed in secrets.json
- Data seeded on startup if no data exists - Dev restriction for employee data
- CORS configured for local development.
- Logging is handled by Serilog (see `logs/`).
- No authentication due to limted time.
- Low test coverage also due to time.
---
