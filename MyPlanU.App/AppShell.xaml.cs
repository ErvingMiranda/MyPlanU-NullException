using MyPlanU.App.Pages;

namespace MyPlanU.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(ConfiguracionPage), typeof(ConfiguracionPage));
        Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
        Routing.RegisterRoute(nameof(PromptyPage), typeof(PromptyPage));
        Routing.RegisterRoute(nameof(CrearEventoPage), typeof(CrearEventoPage));
        Routing.RegisterRoute(nameof(CambiarPasswordPage), typeof(CambiarPasswordPage));
        Routing.RegisterRoute(nameof(AmigosPage), typeof(AmigosPage));
	}
}
