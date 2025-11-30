using MyPlanU.App.Pages;

namespace MyPlanU.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(EventosPage), typeof(EventosPage));
        Routing.RegisterRoute(nameof(ConfiguracionPage), typeof(ConfiguracionPage));
	}
}
