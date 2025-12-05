
using MyPlanU.Backend.Models;

namespace MyPlanU.App;

public partial class App : Application
{
    public static Usuario? CurrentUser { get; set; }

    public App()
    {
        InitializeComponent();

        // Pantalla inicial: Login SIN parámetros
        MainPage = new AppShell();
    }
}
