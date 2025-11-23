# Health (MAUI + WebAPI)

This repository contains:

- HealthMaui is a .NET MAUI front-end for Windows. It shows how to do Patients CRUD and search tasks by using a REST API.

- Health.Api  -  ASP.NET Core WebAPI (in-memory store) that exposes REST endpoints for Patients.

- Health.Core / CLI.Health  -  shared domain and service logic.

Summary

-------

The task needed the implementation of a RESTful WebAPI for one entity and also its integration into the front end. This project includes Patients with these following endpoints (located at `/api/patients`):

- `POST /api/patients`  -  Create a patient.

- `GET /api/patients`  -  Read all patients.

- `GET /api/patients/{id}`  -  Read a patient by id.

- `PUT /api/patients/{id}`  -  Update a patient.

- `DELETE /api/patients/{id}`  -  Delete a patient.

- `GET /api/patients/search?q=...`  -  Search patients by name or address.

How to run (development)

------------------------

1. Start the API (keep this terminal open):

```powershell

cd C:\Users\rupp2\source\repos\Health

dotnet run --project .\Health.Api\ --urls http://localhost:7009

```

2. Run the MAUI Windows exe (in a second terminal):

```powershell

& 'C:\Users\rupp2\source\repos\Health\HealthMaui\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\HealthMaui.exe'

```

The app is used for creating, reading, updating, deleting and searching patients. By default, the MAUI app calls the API at `http://localhost:7009`.

How to test the API manually (PowerShell examples)

-------------------------------------------------

- List all patients:

```powershell

Invoke-RestMethod -Uri http://localhost:7009/api/patients -Method Get

```

- Create a patient:

```powershell

# Health (MAUI + WebAPI)

This repository contains:

- `HealthMaui` — a .NET MAUI front-end for Windows demonstrating Patients CRUD and search via a REST API.
- `Health.Api` — an ASP.NET Core WebAPI (in-memory store) that exposes REST endpoints for Patients.
- `Health.Core` / `CLI.Health` — shared domain and service logic.

## Summary

The assignment required a RESTful WebAPI for one entity and its integration into the front end. This project implements Patients with these endpoints (under `/api/patients`):

- `POST /api/patients` — Create a patient.
- `GET /api/patients` — Read all patients.
- `GET /api/patients/{id}` — Read a patient by id.
- `PUT /api/patients/{id}` — Update a patient.
- `DELETE /api/patients/{id}` — Delete a patient.
- `GET /api/patients/search?q=...` — Search patients by name or address.

## How to run (development)

1. Start the API (keep this terminal open):

```powershell
cd C:\Users\rupp2\source\repos\Health
dotnet run --project .\Health.Api\ --urls http://localhost:7009
```

2. Run the MAUI Windows app

Recommended (publish + run):

```powershell
# Publish the MAUI app (creates a runnable exe and required assets)
dotnet publish .\HealthMaui\HealthMaui.csproj -c Debug -f net8.0-windows10.0.19041.0 -r win10-x64 --self-contained false

# Run the published exe (example path)
& 'C:\Users\rupp2\source\repos\Health\HealthMaui\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\publish\HealthMaui.exe'
```

Alternative (framework-dependent DLL):

```powershell
dotnet C:\Users\rupp2\source\repos\Health\HealthMaui\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\HealthMaui.dll
```

Always start the API first. If the app looks like it is using a local store, rebuild/publish the MAUI project so the exe matches your latest code.

## How to test the API manually (PowerShell examples)

- List all patients:

```powershell
Invoke-RestMethod -Uri http://localhost:7009/api/patients -Method Get
```

- Create a patient:

```powershell
$body = @{ id = ([guid]::Empty); firstName = 'Alice'; lastName = 'Sample'; birthDate = '1990-01-01'; address = '1 Demo St'; race = 'Unknown'; gender = 'Unknown'; notes = @() } | ConvertTo-Json -Depth 10
Invoke-RestMethod -Uri http://localhost:7009/api/patients -Method Post -Body $body -ContentType 'application/json'
```

- Get by id (replace `{id}`):

```powershell
Invoke-RestMethod -Uri http://localhost:7009/api/patients/{id} -Method Get
```

- Update (PUT): send a full PatientDto JSON to `/api/patients/{id}`.

- Search:

```powershell
Invoke-RestMethod -Uri "http://localhost:7009/api/patients/search?q=alice" -Method Get
```

## Notes and tips

- The API uses an in-memory store (ConcurrentDictionary) for demo purposes. Data will be lost when the host stops.
- The MAUI app is wired to the API in `HealthMaui/MauiProgram.cs` by registering an `ApiPatientRepository`. To revert to the original in-memory behavior, change the `IPatientRepository` registration in `MauiProgram.cs` back to `InMemoryPatientRepository`.
- The project intentionally avoids authentication/authorization and HTTPS for assignment simplicity.

## Next recommended improvements (optional)

- Add EF Core + SQLite for persistence across restarts.
- Add unit and integration tests for the API (WebApplicationFactory / xUnit).
