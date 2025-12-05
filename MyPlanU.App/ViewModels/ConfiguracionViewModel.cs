using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyPlanU.App.ViewModels;

public partial class ConfiguracionViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isNotificacionesEnabled;

    [ObservableProperty]
    private bool isModoOscuroEnabled;

    public ConfiguracionViewModel()
    {
        // Inicializar estado (podría venir de preferencias)
        IsModoOscuroEnabled = Application.Current?.UserAppTheme == AppTheme.Dark;
    }

    partial void OnIsModoOscuroEnabledChanged(bool value)
    {
        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        }
    }

    [RelayCommand]
    private async Task EditarPerfilAsync()
    {
        await Application.Current.MainPage.DisplayAlert("Editar Perfil", "Esta funcionalidad estará disponible próximamente.", "OK");
    }

    [RelayCommand]
    private async Task CambiarPasswordAsync()
    {
        await Shell.Current.GoToAsync(nameof(Pages.CambiarPasswordPage));
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        bool confirm = await Application.Current.MainPage.DisplayAlert("Cerrar Sesión", "¿Estás seguro de que deseas salir?", "Sí", "No");
        if (confirm)
        {
            // Aquí se podría limpiar datos de sesión si los hubiera
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
