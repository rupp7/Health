using CLI.Health.Application;
using CLI.Health.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace HealthMaui.Pages;

[QueryProperty(nameof(PatientId), "PatientId")]
public partial class PatientEditPage : ContentPage
{
    private readonly PatientService _service;
    private Patient? _patient;
    
    public string? PatientId { get; set; }

    public PatientEditPage()
    {
        InitializeComponent();
        _service = App.Services!.GetRequiredService<PatientService>();
        // populate pickers from enums
        RacePicker.ItemsSource = Enum.GetNames(typeof(Race));
        GenderPicker.ItemsSource = Enum.GetNames(typeof(Gender));
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            _patient = null;
            if (!string.IsNullOrEmpty(PatientId) && Guid.TryParse(PatientId, out var guid))
            {
                _patient = await _service.GetAsync(guid);
            }

            if (_patient is null)
            {
                BirthDate.Date = DateTime.Today;
                RacePicker.SelectedIndex = 0;
                GenderPicker.SelectedIndex = 0;
                NotesList.ItemsSource = null;
                return;
            }

            FirstName.Text = _patient.FirstName;
            LastName.Text = _patient.LastName;
            Address.Text = _patient.Address;
            BirthDate.Date = new DateTime(_patient.BirthDate.Year, _patient.BirthDate.Month, _patient.BirthDate.Day);
            RacePicker.SelectedIndex = (int)_patient.Race;
            GenderPicker.SelectedIndex = (int)_patient.Gender;

            // populate notes list
            NotesList.ItemsSource = _patient.Notes.Select(n => new {
                n.Diagnoses,
                n.Prescriptions,
                FreeText = n.FreeText,
                CreatedDisplay = n.CreatedUtc.ToLocalTime().ToString("g")
            }).ToList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load patient: {ex.Message}", "OK");
        }
    }

    async void SaveClicked(object sender, EventArgs e)
    {
        try
        {
            // basic validation
            if (string.IsNullOrWhiteSpace(FirstName.Text) || string.IsNullOrWhiteSpace(LastName.Text))
            {
                await DisplayAlert("Validation", "First and last name are required.", "OK");
                return;
            }

            var dob = DateOnly.FromDateTime(BirthDate.Date);
            if (dob > DateOnly.FromDateTime(DateTime.Today))
            {
                await DisplayAlert("Validation", "Birthdate cannot be in the future.", "OK");
                return;
            }

            var race = (Race)Math.Clamp(RacePicker.SelectedIndex, 0, Enum.GetValues(typeof(Race)).Length - 1);
            var gen = (Gender)Math.Clamp(GenderPicker.SelectedIndex, 0, Enum.GetValues(typeof(Gender)).Length - 1);

            if (_patient is null)
            {
                await _service.CreateAsync(FirstName.Text ?? "", LastName.Text ?? "", dob, Address.Text ?? "", race, gen);
                await Shell.Current.GoToAsync("..");
                return;
            }

            _patient.FirstName = FirstName.Text ?? "";
            _patient.LastName = LastName.Text ?? "";
            _patient.Address = Address.Text ?? "";
            _patient.BirthDate = dob;
            _patient.Race = race;
            _patient.Gender = gen;
            await _service.UpdateAsync(_patient);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save patient: {ex.Message}", "OK");
        }
    }

    async void DeleteClicked(object sender, EventArgs e)
    {
        if (_patient is null) { await DisplayAlert("Delete", "No patient to delete.", "OK"); return; }
        var ok = await DisplayAlert("Delete", "Are you sure you want to delete this patient?", "Yes", "No");
        if (!ok) return;
        try
        {
            await _service.DeleteAsync(_patient.Id);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to delete patient: {ex.Message}", "OK");
        }
    }

    async void AddNoteClicked(object sender, EventArgs e)
    {
            try
            {
                if (_patient is null)
                {
                    await DisplayAlert("Save patient first", "Create the patient, then add a note.", "OK");
                    return;
                }

                var note = new MedicalNote
                {
                    PatientId = _patient.Id,
                    Diagnoses = Dx.Text ?? string.Empty,
                    Prescriptions = Rx.Text ?? string.Empty,
                    FreeText = NoteText.Text ?? string.Empty
                };
                _patient.AddNote(note);
                await _service.UpdateAsync(_patient); // persist change

                // refresh UI
                NotesList.ItemsSource = _patient.Notes.Select(n => new {
                    n.Diagnoses,
                    n.Prescriptions,
                    FreeText = n.FreeText,
                    CreatedDisplay = n.CreatedUtc.ToLocalTime().ToString("g")
                }).ToList();

                Dx.Text = Rx.Text = NoteText.Text = string.Empty;
                await DisplayAlert("Note added", "Medical note saved.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to add note: {ex.Message}", "OK");
            }
    }
}
