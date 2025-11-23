using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace HealthMaui;

public partial class App : Application
{
    private readonly ILogger<App> _logger;
    public static IServiceProvider? Services { get; internal set; }

    public App(ILogger<App> logger)
    {
        _logger = logger;
        try
        {
            _logger.LogInformation("Starting application initialization");
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Debug.WriteLine($"Unhandled Exception: {e.ExceptionObject}");
            };

            _logger.LogInformation("Initializing AppShell as MainPage");
            // Use AppShell directly so Shell navigation and back stack behave correctly
            MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application initialization failed");
            MainPage = new ContentPage
            {
                Content = new VerticalStackLayout
                {
                    Children = 
                    {
                        new Label 
                        {
                            Text = "Application initialization failed",
                            FontSize = 24,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label 
                        {
                            Text = ex.Message,
                            FontSize = 16,
                            HorizontalOptions = LayoutOptions.Center,
                            TextColor = Colors.Red
                        }
                    },
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    Padding = new Thickness(20)
                }
            };
        }
    }

    protected override void OnStart()
    {
        try
        {
            _logger.LogInformation("Application OnStart called");
            base.OnStart();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnStart");
        }
    }
}
