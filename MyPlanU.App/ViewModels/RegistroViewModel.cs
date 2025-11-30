using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public partial class RegistroViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string nombre;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    public RegistroViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        var errorCorreo = ValidationHelper.ValidarCorreo(Email);
        if (!string.IsNullOrEmpty(errorCorreo))
        {
            await Shell.Current.DisplayAlert("Error", errorCorreo, "OK");
            return;
        }

        var errorPass = ValidationHelper.ValidarContrasena(Password);
        if (!string.IsNullOrEmpty(errorPass))
        {
            await Shell.Current.DisplayAlert("Error", errorPass, "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Nombre))
        {
            await Shell.Current.DisplayAlert("Error", "El nombre es requerido", "OK");
            return;
        }

        var nuevoUsuario = new Usuario
        {
            Nombre = Nombre,
            Email = Email,
            ContrasenaHash = Password, // En producción usar hash real
            FechaRegistro = DateTime.Now,
            EstadoCuenta = "Activo"
        };

        bool exito = await _authService.RegisterAsync(nuevoUsuario);

        if (exito)
        {
            await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
            await Shell.Current.GoToAsync(".."); // Volver al Login
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "El email ya está registrado", "OK");
        }
    }
}
