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

$id = ([guid]::Empty);
$firstName = 'Alice';
$lastName = 'Sample';
$birthDate = '1990-01-01';
$address = '1 Demo St';
$race = 'Unknown';
$gender = 'Unknown';
$notes = @() }

| ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri http://localhost:7009/api/patients -Method Post -Body $body -ContentType 'application/json'

```

- Get by id (replace {id}):

```powershell

Invoke-RestMethod -Uri http://localhost:7009/api/patients/{id} -Method Get

```

- Update (PUT):

```powershell

# send full PatientDto JSON to PUT /api/patients/{id}

```

- Search:

```powershell

Invoke-RestMethod -Uri "http://localhost:7009/api/patients/search?q=alice" -Method Get

```

Notes and tips

--------------

The API relies on an in-memory store (ConcurrentDictionary) for demonstration aims. The data will disappear when the host ceases to operate.

The MAUI application is connected to the API in `HealthMaui/MauiProgram.cs` through registering an `ApiPatientRepository`. If you want to return to the initial in-memory function, adjust the `IPatientRepository` registration back to `InMemoryPatientRepository` in `MauiProgram.cs`.

- The project intentionally avoids authentication/authorization and HTTPS for assignment simplicity.

Next recommended improvements (optional)

--------------------------------------

- Add EF Core + SQLite for persistence across restarts.

- Add unit and integration tests for the API (WebApplicationFactory / xUnit).
