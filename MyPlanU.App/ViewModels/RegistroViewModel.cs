using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public partial class RegistroViewModel : ObservableObject
{
    private readonly AuthService _authService;

    // PROPIEDADES DEL FORMULARIO
    [ObservableProperty]
    private string nombre;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string preguntaSeguridad;

    [ObservableProperty]
    private string preguntaPersonalizada;

    [ObservableProperty]
    private string respuestaSeguridad;

    [ObservableProperty]
    private bool isCustomQuestionVisible;

    public List<string> PreguntasDisponibles { get; } = new List<string>
    {
        "¿Cuál es tu comida favorita?",
        "¿Cuál es tu color favorito?",
        "¿Cómo se llama tu mejor amigo?",
        "Escribir mi propia pregunta..."
    };

    public RegistroViewModel(AuthService authService)
    {
        _authService = authService;
    }

    partial void OnPreguntaSeguridadChanged(string value)
    {
        IsCustomQuestionVisible = value == "Escribir mi propia pregunta...";
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        // VALIDACIONES BÁSICAS
        if (string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(Email)  ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(PreguntaSeguridad) ||
            string.IsNullOrWhiteSpace(RespuestaSeguridad))
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "Por favor completa todos los campos, incluyendo la pregunta de seguridad.",
                "Aceptar");
            return;
        }

        string preguntaFinal = PreguntaSeguridad;
        if (IsCustomQuestionVisible)
        {
            if (string.IsNullOrWhiteSpace(PreguntaPersonalizada))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor escribe tu pregunta personalizada.", "OK");
                return;
            }
            preguntaFinal = PreguntaPersonalizada;
        }

        var nuevoUsuario = new Usuario
        {
            Nombre = Nombre,
            Email = Email,
            ContrasenaHash = Password, // En producción, hashear esto
            PreguntaSeguridad = preguntaFinal,
            RespuestaSeguridad = RespuestaSeguridad,
            FechaRegistro = DateTime.Now
        };

        bool exito = await _authService.RegisterAsync(nuevoUsuario);

        if (exito)
        {
            await Shell.Current.DisplayAlert(
                "¡Registrado!",
                "Tu cuenta fue creada correctamente 💜",
                "Continuar");

            // Navegar atrás (volver al Login)
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert(
                "Error",
                "El correo ya está registrado o hubo un problema.",
                "Aceptar");
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}


