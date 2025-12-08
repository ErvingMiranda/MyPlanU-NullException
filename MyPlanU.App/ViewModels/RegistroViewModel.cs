using System.Windows.Input;
using Microsoft.Maui.Controls;

using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public class RegistroViewModel
{
    private readonly AuthService _authService;

    // PROPIEDADES DEL FORMULARIO
    public string Nombre { get; set; }
    public string Email  { get; set; }
    public string Password { get; set; }
    public string PreguntaSeguridad { get; set; }
    public string RespuestaSeguridad { get; set; }

    public List<string> PreguntasDisponibles { get; } = new List<string>
    {
        "¿Cuál es el nombre de tu primera mascota?",
        "¿En qué ciudad naciste?",
        "¿Cuál es el nombre de tu madre?",
        "¿Cuál es tu comida favorita?",
        "¿Cuál es el nombre de tu mejor amigo de la infancia?"
    };

    // COMMAND PARA EL BOTÓN REGISTRARSE
    public ICommand RegistrarCommand { get; }
    public ICommand GoBackCommand { get; }

    public RegistroViewModel(AuthService authService)
    {
        _authService = authService;
        // Command de MAUI (nativo, sin librerías extra)
        RegistrarCommand = new Command(async () => await Registrar());
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private async Task Registrar()
    {
        // VALIDACIONES BÁSICAS
        if (string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(Email)  ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(PreguntaSeguridad) ||
            string.IsNullOrWhiteSpace(RespuestaSeguridad))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "Por favor completa todos los campos, incluyendo la pregunta de seguridad.",
                "Aceptar");
            return;
        }

        var nuevoUsuario = new Usuario
        {
            Nombre = Nombre,
            Email = Email,
            ContrasenaHash = Password, // En producción, hashear esto
            PreguntaSeguridad = PreguntaSeguridad,
            RespuestaSeguridad = RespuestaSeguridad,
            FechaRegistro = DateTime.Now,
            EstadoCuenta = "Activo"
        };

        bool exito = await _authService.RegisterAsync(nuevoUsuario);

        if (exito)
        {
            await Application.Current.MainPage.DisplayAlert(
                "¡Registrado!",
                "Tu cuenta fue creada correctamente 💜",
                "Continuar");

            // Navegar atrás (volver al Login)
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "El correo ya está registrado o hubo un problema.",
                "Aceptar");
        }
    }
}


