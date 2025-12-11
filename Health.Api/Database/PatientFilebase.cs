using System.Text.Json;
using Health.Api.Models;

namespace Health.Api.Database;

/// <summary>
/// File-based persistence for Patients. Stores each patient as a JSON file.
/// Singleton pattern ensures a single instance manages the store.
/// </summary>
public sealed class PatientFilebase
{
    private string _root;
    private string _patientsRoot;
    private static PatientFilebase _instance;

    public static PatientFilebase Current
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PatientFilebase();
            }
            return _instance;
        }
    }

    private PatientFilebase()
    {
        // Store patients in %TEMP%/Health/Patients
        _root = Path.Combine(Path.GetTempPath(), "Health");
        _patientsRoot = Path.Combine(_root, "Patients");
        
        // Ensure directories exist
        Directory.CreateDirectory(_patientsRoot);
    }

    /// <summary>
    /// Get the next available integer ID based on existing patient files.
    /// </summary>
    public int GetNextPatientKey()
    {
        try
        {
            var root = new DirectoryInfo(_patientsRoot);
            var files = root.GetFiles("*.json");
            if (files.Length == 0) return 1;

            var ids = files
                .Select(f => Path.GetFileNameWithoutExtension(f.Name))
                .Where(name => int.TryParse(name, out _))
                .Select(int.Parse)
                .ToList();

            return ids.Any() ? ids.Max() + 1 : 1;
        }
        catch
        {
            return 1;
        }
    }

    /// <summary>
    /// Get all patients from JSON files.
    /// </summary>
    public List<PatientDto> GetAllPatients()
    {
        var patients = new List<PatientDto>();
        try
        {
            var root = new DirectoryInfo(_patientsRoot);
            if (!root.Exists) return patients;

            foreach (var file in root.GetFiles("*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file.FullName);
                    var patient = JsonSerializer.Deserialize<PatientDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (patient != null)
                    {
                        patients.Add(patient);
                    }
                }
                catch
                {
                    // Skip malformed files
                }
            }
        }
        catch
        {
            // Return empty list if directory doesn't exist or can't be read
        }
        return patients;
    }

    /// <summary>
    /// Get a patient by ID (as a string key for file naming).
    /// </summary>
    public PatientDto? GetPatient(Guid id)
    {
        try
        {
            var path = Path.Combine(_patientsRoot, $"{id}.json");
            if (!File.Exists(path)) return null;

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<PatientDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Add or update a patient. If ID is empty, assign a new one.
    /// </summary>
    public PatientDto AddOrUpdate(PatientDto patient)
    {
        try
        {
            // If ID is empty, generate a new one
            if (patient.Id == Guid.Empty)
            {
                patient.Id = Guid.NewGuid();
            }

            var path = Path.Combine(_patientsRoot, $"{patient.Id}.json");
            var json = JsonSerializer.Serialize(patient, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);

            return patient;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to persist patient {patient.Id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Delete a patient by ID.
    /// </summary>
    public bool DeletePatient(Guid id)
    {
        try
        {
            var path = Path.Combine(_patientsRoot, $"{id}.json");
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
