using System;
using System.Collections.Generic;

namespace Health.Api.Models;

public sealed class MedicalNoteDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public string? Diagnoses { get; set; }
    public string? Prescriptions { get; set; }
    public string? FreeText { get; set; }
}

public sealed class PatientDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Race { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public List<MedicalNoteDto> Notes { get; set; } = new();
}
