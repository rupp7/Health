using CLI.Health.Application;

namespace HealthMaui.Pages;

public partial class AppointmentsPage : ContentPage
{
    private readonly AppointmentService _apptSvc;
    private readonly PatientService _patSvc;
    private readonly PhysicianService _docSvc;

    public AppointmentsPage(AppointmentService a, PatientService p, PhysicianService d)
    { InitializeComponent(); _apptSvc = a; _patSvc = p; _docSvc = d; }

    public AppointmentsPage() : this(
    (AppointmentService)App.Services!.GetRequiredService(typeof(AppointmentService)),
    (PatientService)App.Services!.GetRequiredService(typeof(PatientService)),
    (PhysicianService)App.Services!.GetRequiredService(typeof(PhysicianService))) {}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var appts = await _apptSvc.ListAsync();
        var patients = (await _patSvc.ListAsync()).ToDictionary(p => p.Id);
        var docs = (await _docSvc.ListAsync()).ToDictionary(d => d.Id);

        List.ItemsSource = appts.Select(a => new {
            Raw = a,
            Display = $"{patients[a.PatientId].LastName}, {patients[a.PatientId].FirstName} @ {docs[a.PhysicianId].LastName} — {a.Date:MM/dd/yyyy} {a.Start:hh\\:mm} ({a.Duration.TotalMinutes}m)"
        }).ToList();
    }

    private async void AddClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(AppointmentEditPage));

    private async void DeleteInvoked(object sender, EventArgs e)
    {
        if (((SwipeItem)sender).BindingContext is null) return;
        dynamic d = ((SwipeItem)sender).BindingContext;
        var id = d.Raw.Id;
        await _apptSvc.DeleteAsync(id);
        OnAppearing();
    }
}
