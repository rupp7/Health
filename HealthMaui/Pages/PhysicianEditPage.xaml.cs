using CLI.Health.Application;
using CLI.Health.Domain;

namespace HealthMaui.Pages;

[QueryProperty(nameof(Model), "Model")]
public partial class PhysicianEditPage : ContentPage
{
    public object? Model { get; set; }
    private readonly PhysicianService _svc;
    private Physician? _doc;

    public PhysicianEditPage(PhysicianService svc) { InitializeComponent(); _svc = svc; }

    public PhysicianEditPage() : this(
    (PhysicianService)App.Services!.GetRequiredService(typeof(PhysicianService))) {}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _doc = Model as Physician;
        if (_doc != null)
        {
            FirstName.Text = _doc.FirstName;
            LastName.Text  = _doc.LastName;
            LicenseNumber.Text = _doc.LicenseNumber;
            GraduationDate.Date = new DateTime(_doc.GraduationDate.Year, _doc.GraduationDate.Month, _doc.GraduationDate.Day);
            SpecializationsCsv.Text = string.Join(", ", _doc.Specializations);
        }
        else
        {
            GraduationDate.Date = DateTime.Today;
        }
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        var grad = DateOnly.FromDateTime(GraduationDate.Date);
        var specs = (SpecializationsCsv.Text ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim());

        if (_doc is null)
        {
            await _svc.CreateAsync(FirstName.Text ?? "", LastName.Text ?? "", LicenseNumber.Text ?? "", grad, specs);
        }
        else
        {
            _doc.FirstName = FirstName.Text ?? "";
            _doc.LastName  = LastName.Text ?? "";
            _doc.LicenseNumber = LicenseNumber.Text ?? "";
            _doc.GraduationDate = grad;
            _doc.Specializations = specs.ToList();
            await _svc.UpdateAsync(_doc);
        }
        await Shell.Current.GoToAsync("..");
    }

    private async void DeleteClicked(object sender, EventArgs e)
    {
        if (_doc is null) { await Shell.Current.DisplayAlert("Nothing to delete", "This physician hasn't been created yet.", "OK"); return; }
        var ok = await DisplayAlert("Confirm", "Delete this physician?", "Delete", "Cancel");
        if (!ok) return;
        await _svc.DeleteAsync(_doc.Id);
        await Shell.Current.GoToAsync("..");
    }
}
