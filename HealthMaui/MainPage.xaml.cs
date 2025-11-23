namespace HealthMaui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnShowPatientListClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Health Management", "Patient List will be implemented here.", "OK");
    }
}
