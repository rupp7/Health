using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HealthMaui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		try
		{
			this.InitializeComponent();
		}
		catch (Exception ex)
		{
			try { System.IO.File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "maui-win-init-exception.txt"), ex.ToString()); } catch {}
			throw;
		}
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

