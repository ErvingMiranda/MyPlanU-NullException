using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.App.Pages;

namespace MyPlanU.App.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    public LoginViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task GoToRegistroAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegistroPage));
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Ingrese email y contraseña", "OK");
            return;
        }

        var usuario = await _authService.LoginAsync(Email, Password);
        if (usuario != null)
        {
            // Navigate to EventosPage passing the user
            var navigationParameter = new Dictionary<string, object>
            {
                { "Usuario", usuario }
            };
            await Shell.Current.GoToAsync(nameof(EventosPage), navigationParameter);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Credenciales inválidas", "OK");
        }
    }
}
