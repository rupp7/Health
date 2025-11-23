namespace HealthMaui.Pages;

public partial class PatientsPage : ContentPage
{
    private readonly CLI.Health.Application.PatientService _svc;

    public PatientsPage()
    {
        InitializeComponent();
        _svc = (CLI.Health.Application.PatientService)App.Services!.GetRequiredService(typeof(CLI.Health.Application.PatientService));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPatients();
    }

    private async Task LoadPatients()
    {
        var list = await _svc.ListAsync();
        PatientsList.ItemsSource = list.Select(p => new ViewModels.PatientListItem(p)).ToList();
    }

    private async void AddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PatientEditPage));
    }

    private async void PatientsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is ViewModels.PatientListItem item)
        {
            var id = item.Raw.Id;
            await Shell.Current.GoToAsync($"{nameof(PatientEditPage)}?PatientId={id}");
            PatientsList.SelectedItem = null;
        }
    }

    private async void DeleteInvoked(object sender, EventArgs e)
    {
        if (((SwipeItem)sender).BindingContext is ViewModels.PatientListItem item)
        {
            await _svc.DeleteAsync(item.Raw.Id);
            await LoadPatients();
        }
    }
}
