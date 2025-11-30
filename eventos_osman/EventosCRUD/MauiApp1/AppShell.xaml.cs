using MauiApp1.Views;

namespace MauiApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EventosPage), typeof(EventosPage));
            Routing.RegisterRoute(nameof(EditarEventoPage), typeof(EditarEventoPage));
            Routing.RegisterRoute(nameof(FormularioEdicionPage), typeof(FormularioEdicionPage));

        }
    }
}   