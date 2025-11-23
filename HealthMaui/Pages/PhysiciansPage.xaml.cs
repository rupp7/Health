using CLI.Health.Application;

namespace HealthMaui.Pages;

public partial class PhysiciansPage : ContentPage
{
    private readonly PhysicianService _svc;
    public PhysiciansPage(PhysicianService svc) { InitializeComponent(); _svc = svc; }

    public PhysiciansPage() : this(
    (PhysicianService)App.Services!.GetRequiredService(typeof(PhysicianService))) {}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        List.ItemsSource = (await _svc.ListAsync())
          .Select(d => new { Raw = d, Display = $"Dr. {d.LastName}, {d.FirstName} • Lic {d.LicenseNumber}" })
          .ToList();
    }

    private async void AddClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(PhysicianEditPage));

    private async void OnSelected(object sender, SelectionChangedEventArgs e)
    {
    var item = e.CurrentSelection.FirstOrDefault();
    if (item is null) return;
    dynamic d = item;
    await Shell.Current.GoToAsync(nameof(PhysicianEditPage), new Dictionary<string, object> { { "Model", d.Raw } });
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void DeleteInvoked(object sender, EventArgs e)
    {
        var cp = (sender as SwipeItem)?.CommandParameter;
        if (cp is not null)
        {
            dynamic d = cp;
            await _svc.DeleteAsync(d.Raw.Id);
            OnAppearing();
        }
    }
}
