using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public partial class EditarPerfilViewModel : ObservableObject
{
    private readonly AuthService _authService;
    private Usuario _currentUser;

    [ObservableProperty]
    private string nombre;

    [ObservableProperty]
    private string apellido;

    [ObservableProperty]
    private string apodo;

    [ObservableProperty]
    private string pais;

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

    public EditarPerfilViewModel(AuthService authService)
    {
        _authService = authService;
        LoadUserData();
    }

    partial void OnPreguntaSeguridadChanged(string value)
    {
        IsCustomQuestionVisible = value == "Escribir mi propia pregunta...";
    }

    private void LoadUserData()
    {
        if (App.CurrentUser != null)
        {
            _currentUser = App.CurrentUser;
            Nombre = _currentUser.Nombre;
            Apellido = _currentUser.Apellido;
            Apodo = _currentUser.Apodo;
            Pais = _currentUser.Pais;
            
            // Check if current question is in the list
            if (PreguntasDisponibles.Contains(_currentUser.PreguntaSeguridad))
            {
                PreguntaSeguridad = _currentUser.PreguntaSeguridad;
            }
            else
            {
                // It's a custom question
                PreguntaSeguridad = "Escribir mi propia pregunta...";
                PreguntaPersonalizada = _currentUser.PreguntaSeguridad;
            }
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_currentUser == null) return;

        _currentUser.Nombre = Nombre;
        _currentUser.Apellido = Apellido;
        _currentUser.Apodo = Apodo;
        _currentUser.Pais = Pais;
        
        if (!string.IsNullOrWhiteSpace(PreguntaSeguridad))
        {
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
            _currentUser.PreguntaSeguridad = preguntaFinal;
        }
            
        if (!string.IsNullOrWhiteSpace(RespuestaSeguridad))
            _currentUser.RespuestaSeguridad = RespuestaSeguridad;

        await _authService.UpdateProfileAsync(_currentUser);
        
        await Shell.Current.DisplayAlert("Éxito", "Perfil actualizado", "OK");
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
