namespace MyPlanU;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Aquí puedes cambiar la página inicial cuando ya tengas Login/Home
        MainPage = new AppShell();
    }
}
