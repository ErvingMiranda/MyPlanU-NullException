
namespace MyPlanU.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Pantalla inicial: Login SIN parámetros
        MainPage = new AppShell();
    }
}
