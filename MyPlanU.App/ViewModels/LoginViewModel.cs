using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.App.Pages;

namespace MyPlanU.App.ViewModels;

public partial class LoginViewModel : ObservableObject, IQueryAttributable
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isPasswordHidden = true;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Logout"))
        {
            Email = string.Empty;
            Password = string.Empty;
        }
    }

    public LoginViewModel()
    {
    }

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
            App.CurrentUser = usuario;
            await Shell.Current.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
            // Navigate to EventosPage passing the user
            var navigationParameter = new Dictionary<string, object>
            {
                { "Usuario", usuario }
            };
            await Shell.Current.GoToAsync($"//{nameof(EventosPage)}", navigationParameter);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Credenciales inválidas", "OK");
        }
    }

    [RelayCommand]
    private async Task Exit()
    {
        bool confirm = await Shell.Current.DisplayAlert("Salir", "¿Seguro que quieres salir?", "Sí", "No");
        if (confirm)
        {
            Application.Current.Quit();
        }
    }

    [RelayCommand]
    private async Task ForgotPasswordAsync()
    {
        await Shell.Current.GoToAsync("RecuperarPasswordPage");
    }
}
