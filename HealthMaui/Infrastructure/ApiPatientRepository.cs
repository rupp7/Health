using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Health.Application;
using CLI.Health.Domain;
using HealthMaui.Utilities;

namespace CLI.Health.Infrastructure
{
    // API-backed implementation of IPatientRepository. Uses WebRequestHandler to call Health.Api.
    public sealed class ApiPatientRepository : IPatientRepository
    {
        private readonly WebRequestHandler _http;

        public ApiPatientRepository(WebRequestHandler http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        private sealed class PatientDto
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string BirthDate { get; set; } = string.Empty; // yyyy-MM-dd
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

        public async Task AddAsync(Patient p)
        {
            var dto = MapToDto(p);
            // POST the dto; API accepts the Id we send
            await _http.Post<PatientDto>("/api/patients", dto).ConfigureAwait(false);
        }

        public async Task<Patient?> GetAsync(Guid id)
        {
            try
            {
                var dto = await _http.Get<PatientDto>($"/api/patients/{id}").ConfigureAwait(false);
                return MapToDomain(dto);
            }
            catch
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<Patient>> ListAsync()
        {
            var dtos = await _http.Get<List<PatientDto>>("/api/patients").ConfigureAwait(false);
            return dtos.Select(MapToDomain).ToList();
        }

        public async Task UpdateAsync(Patient p)
        {
            var dto = MapToDto(p);
            await _http.Put($"/api/patients/{p.Id}", dto).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _http.Delete($"/api/patients/{id}").ConfigureAwait(false);
        }

        private static Patient MapToDomain(PatientDto d)
        {
            var birth = DateOnly.FromDateTime(DateTime.Parse(d.BirthDate));
            var p = new Patient
            {
                FirstName = d.FirstName,
                LastName = d.LastName,
                BirthDate = birth,
                Address = d.Address,
                Race = Enum.TryParse<Race>(d.Race, out var r) ? r : Race.Unknown,
                Gender = Enum.TryParse<Gender>(d.Gender, out var g) ? g : Gender.Unknown
            };
            // set init-only Id via reflection because Id is declared in the base class as init-only
            var ret = new Patient
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthDate = p.BirthDate,
                Address = p.Address,
                Race = p.Race,
                Gender = p.Gender,
            };
            var idProp = typeof(CLI.Health.Domain.Entity).GetProperty("Id");
            if (idProp != null)
            {
                idProp.SetValue(ret, d.Id);
            }

            foreach (var n in d.Notes ?? new List<MedicalNoteDto>())
            {
                ret.AddNote(new CLI.Health.Domain.MedicalNote
                {
                    PatientId = d.Id,
                    Diagnoses = n.Diagnoses,
                    Prescriptions = n.Prescriptions,
                    FreeText = n.FreeText
                });
            }

            return ret;
        }

        private static PatientDto MapToDto(Patient p)
        {
            return new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                BirthDate = p.BirthDate.ToString("yyyy-MM-dd"),
                Address = p.Address,
                Race = p.Race.ToString(),
                Gender = p.Gender.ToString(),
                Notes = p.Notes.Select(n => new MedicalNoteDto { Id = n.Id, CreatedUtc = n.CreatedUtc, Diagnoses = n.Diagnoses, Prescriptions = n.Prescriptions, FreeText = n.FreeText }).ToList()
            };
        }
    }
}
