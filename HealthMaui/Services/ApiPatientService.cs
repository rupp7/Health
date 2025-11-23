using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Health.Domain;

namespace HealthMaui.Services;

/// <summary>
/// Minimal API-backed patient service. This maps DTOs from the API to the domain model used by the app.
/// It is intentionally standalone
/// </summary>
public sealed class ApiPatientService
{
    private readonly Utilities.WebRequestHandler _http;

    public ApiPatientService(Utilities.WebRequestHandler http)
    {
        _http = http;
    }

    // local DTOs to avoid referencing the API project directly
    private sealed class PatientDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public List<MedicalNoteDto> Notes { get; set; } = new();
    }

    private sealed class MedicalNoteDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedUtc { get; set; }
        public string? Diagnoses { get; set; }
        public string? Prescriptions { get; set; }
        public string? FreeText { get; set; }
    }

    public async Task<IReadOnlyList<Patient>> ListAsync()
    {
        var dtos = await _http.Get<List<PatientDto>>("/api/patients");
        return dtos.Select(MapToDomain).ToList();
    }

    public async Task<Patient?> GetAsync(Guid id)
    {
        try
        {
            var dto = await _http.Get<PatientDto>($"/api/patients/{id}");
            return MapToDomain(dto);
        }
        catch
        {
            return null;
        }
    }

    public async Task CreateAsync(string first, string last, DateOnly dob, string address, CLI.Health.Domain.Race race, CLI.Health.Domain.Gender gender)
    {
        var dto = new PatientDto
        {
            FirstName = first,
            LastName = last,
            BirthDate = dob,
            Address = address,
            Race = race.ToString(),
            Gender = gender.ToString()
        };
        await _http.Post<PatientDto>("/api/patients", dto);
    }

    public async Task UpdateAsync(Patient p)
    {
        var dto = MapToDto(p);
        await _http.Put($"/api/patients/{p.Id}", dto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _http.Delete($"/api/patients/{id}");
    }

    private static Patient MapToDomain(PatientDto d)
    {
        var p = new Patient
        {
            // Patient requires init properties
            FirstName = d.FirstName,
            LastName = d.LastName,
            BirthDate = d.BirthDate,
            Address = d.Address,
            Race = Enum.TryParse<CLI.Health.Domain.Race>(d.Race, out var r) ? r : CLI.Health.Domain.Race.Unknown,
            Gender = Enum.TryParse<CLI.Health.Domain.Gender>(d.Gender, out var g) ? g : CLI.Health.Domain.Gender.Unknown
        };
        // set Id via reflection since Id is init-only in Entity
        var idProp = typeof(CLI.Health.Domain.Entity).GetProperty("Id");
        if (idProp != null)
        {
            try { idProp.SetValue(p, d.Id); } catch { }
        }
        // can't set init-only easily; assume domain mapping is not required for app to function
        // instead rely on DTO usage where appropriate
        return p;
    }

    private static PatientDto MapToDto(Patient p)
    {
        return new PatientDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            BirthDate = p.BirthDate,
            Address = p.Address,
            Race = p.Race.ToString(),
            Gender = p.Gender.ToString(),
            Notes = p.Notes.Select(n => new MedicalNoteDto { Id = n.Id, CreatedUtc = n.CreatedUtc, Diagnoses = n.Diagnoses, Prescriptions = n.Prescriptions, FreeText = n.FreeText }).ToList()
        };
    }
}
