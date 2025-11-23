using CLI.Health.Application;
using CLI.Health.Infrastructure;
using Microsoft.Extensions.Logging;

namespace HealthMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>();

            // debugging with large logging capabilitie
            builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Trace);

            // NOTE: the file-based logging provider has caused native startup issues
            // in some MAUI/WinUI environments. Omit it to avoid crashes during app
            // startup.

            // Register repositories first
            // Use API-backed repository so PatientService continues to work but calls the external WebAPI.
            builder.Services.AddSingleton(new HealthMaui.Utilities.WebRequestHandler("http://localhost:7009"));
            builder.Services.AddSingleton<IPatientRepository, CLI.Health.Infrastructure.ApiPatientRepository>();
            builder.Services.AddSingleton<IPhysicianRepository, InMemoryPhysicianRepository>();
            builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();

            // Register services that depend on repositories
            builder.Services.AddSingleton<PatientService>();
            builder.Services.AddSingleton<PhysicianService>();
            builder.Services.AddSingleton<AppointmentService>();

            // Register pages last
            builder.Services.AddTransient<Pages.PatientsPage>();
            builder.Services.AddTransient<Pages.PatientEditPage>();
            builder.Services.AddTransient<Pages.PhysiciansPage>();
            builder.Services.AddTransient<Pages.PhysicianEditPage>();
            builder.Services.AddTransient<Pages.AppointmentsPage>();
            builder.Services.AddTransient<Pages.AppointmentEditPage>();

            var app = builder.Build();
            App.Services = app.Services;
            return app;
        }
        catch (Exception ex)
        {
            try { System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "maui-createapp-exception.txt"), ex.ToString()); } catch {}
            throw;
        }
    }
}
