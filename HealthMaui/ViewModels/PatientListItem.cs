using CLI.Health.Domain;

namespace HealthMaui.ViewModels;

public class PatientListItem
{
    public Guid Id => Raw.Id;
    public Patient Raw { get; }
    public string Display { get; }

    public PatientListItem(Patient patient)
    {
        Raw = patient;
        Display = $"{patient.LastName}, {patient.FirstName} • DOB {patient.BirthDate:MM/dd/yyyy}";
    }
}