using System.Collections.ObjectModel;
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
    private bool isPasswordHidden = true;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [ObservableProperty]
    private string preguntaSeguridad;

    [ObservableProperty]
    private string preguntaPersonalizada;

    [ObservableProperty]
    private string respuestaSeguridad;

    [ObservableProperty]
    private bool isCustomQuestionVisible;

    public ObservableCollection<string> PreguntasDisponibles { get; } = new ObservableCollection<string>
    {
        "¿Cuál es tu comida favorita?",
        "¿Cuál es tu color favorito?",
        "¿Cómo se llama tu mejor amigo?",
        "Escribir mi propia pregunta...",
        "Eliminar..."
    };

    public RegistroViewModel(AuthService authService)
    {
        _authService = authService;
    }

    partial void OnPreguntaSeguridadChanged(string value)
    {
        if (value == "Escribir mi propia pregunta...")
        {
            IsCustomQuestionVisible = true;
        }
        else if (value == "Eliminar...")
        {
            IsCustomQuestionVisible = false;
            EliminarPreguntaDisponibleAsync();
            PreguntaSeguridad = null;
        }
        else
        {
            IsCustomQuestionVisible = false;
        }
    }

    private async void EliminarPreguntaDisponibleAsync()
    {
        var questionsToDelete = PreguntasDisponibles
            .Where(q => q != "Escribir mi propia pregunta..." && q != "Eliminar...")
            .ToArray();

        if (questionsToDelete.Length == 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "No hay preguntas para eliminar.", "OK");
            return;
        }

        string result = await Shell.Current.DisplayActionSheet("Eliminar pregunta", "Cancelar", null, questionsToDelete);

        if (result != "Cancelar" && !string.IsNullOrEmpty(result))
        {
            PreguntasDisponibles.Remove(result);
        }
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

        string errorPass = ValidationHelper.ValidarContrasena(Password);
        if (!string.IsNullOrEmpty(errorPass))
        {
            await Shell.Current.DisplayAlert("Error", errorPass, "OK");
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


