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
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Nombre))
        {
            await Shell.Current.DisplayAlert("Error", "Por favor complete todos los campos", "OK");
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
