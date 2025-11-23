namespace HealthMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("PatientEditPage", typeof(Pages.PatientEditPage));
        Routing.RegisterRoute(nameof(Pages.PhysicianEditPage), typeof(Pages.PhysicianEditPage));
        Routing.RegisterRoute(nameof(Pages.AppointmentEditPage), typeof(Pages.AppointmentEditPage));
    }
}
