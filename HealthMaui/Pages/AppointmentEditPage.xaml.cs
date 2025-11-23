using CLI.Health.Application;

namespace HealthMaui.Pages;

public partial class AppointmentEditPage : ContentPage
{
    private readonly AppointmentService _apptSvc;
    private readonly PatientService _patSvc;
    private readonly PhysicianService _docSvc;

    public AppointmentEditPage(AppointmentService a, PatientService p, PhysicianService d)
    { InitializeComponent(); _apptSvc = a; _patSvc = p; _docSvc = d; }

    public AppointmentEditPage() : this(
    (AppointmentService)App.Services!.GetRequiredService(typeof(AppointmentService)),
    (PatientService)App.Services!.GetRequiredService(typeof(PatientService)),
    (PhysicianService)App.Services!.GetRequiredService(typeof(PhysicianService))) {}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var pats = await _patSvc.ListAsync();
        var docs = await _docSvc.ListAsync();

        PatientPicker.ItemsSource  = pats.Select(p => new { Raw = p, Label = $"{p.LastName}, {p.FirstName}" }).ToList();
        PatientPicker.ItemDisplayBinding = new Binding("Label");

        PhysicianPicker.ItemsSource = docs.Select(d => new { Raw = d, Label = $"Dr. {d.LastName} ({d.LicenseNumber})" }).ToList();
        PhysicianPicker.ItemDisplayBinding = new Binding("Label");

        DatePicker.Date = DateTime.Today;
        StartTime.Time  = new TimeSpan(8, 0, 0);
        DurationMins.Text = "60";
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        try
        {
            Error.IsVisible = false;
            if (PatientPicker.SelectedItem is null || PhysicianPicker.SelectedItem is null)
                throw new InvalidOperationException("Select patient and physician.");

            dynamic p = PatientPicker.SelectedItem!;
            dynamic d = PhysicianPicker.SelectedItem!;
            var date  = DateOnly.FromDateTime(DatePicker.Date);
            var start = TimeOnly.FromTimeSpan(StartTime.Time);
            var mins  = int.TryParse(DurationMins.Text, out var m) ? m : 0;
            if (mins <= 0) throw new InvalidOperationException("Duration must be > 0");

            // Enforces Mon–Fri 08:00–17:00 and prevents double-booking (your service rules)
            await _apptSvc.ScheduleAsync(p.Raw.Id, d.Raw.Id, date, start, TimeSpan.FromMinutes(mins));

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Error.Text = ex.Message;
            Error.IsVisible = true;
        }
    }
}
