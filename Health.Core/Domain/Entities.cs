using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI.Health.Domain
{
    public abstract class Entity
    {
        public Guid Id { get; init; } = Guid.NewGuid();
    }

    public sealed class Patient : Entity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required string Address { get; set; }
        public Race Race { get; set; } = Race.Unknown;
        public Gender Gender { get; set; } = Gender.Unknown;

        private readonly List<MedicalNote> _notes = new();
        public IReadOnlyList<MedicalNote> Notes => _notes;
        public void AddNote(MedicalNote note) => _notes.Add(note);
    }

    public sealed class Physician : Entity
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string LicenseNumber { get; set; }
        public required DateOnly GraduationDate { get; set; }
        public required List<string> Specializations { get; set; } = new();
    }

    public sealed class MedicalNote : Entity
    {
        public required Guid PatientId { get; set; }
        public Guid? PhysicianId { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public string? Diagnoses { get; set; }
        public string? Prescriptions { get; set; }
        public string? FreeText { get; set; }
    }

    public sealed class Appointment : Entity
    {
        public required Guid PatientId { get; set; }
        public required Guid PhysicianId { get; set; }
        public required DateOnly Date { get; set; }
        public required TimeOnly Start { get; set; }
        public required TimeSpan Duration { get; set; }
        public TimeOnly End => Start.Add(Duration);
    }
}
