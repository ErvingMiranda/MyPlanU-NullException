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
    private string zonaHoraria;

    [ObservableProperty]
    private string preguntaSeguridad;

    [ObservableProperty]
    private string respuestaSeguridad;

    public List<string> PreguntasDisponibles { get; } = new List<string>
    {
        "¿Cuál es el nombre de tu primera mascota?",
        "¿En qué ciudad naciste?",
        "¿Cuál es el nombre de tu madre?",
        "¿Cuál es tu comida favorita?",
        "¿Cuál es el nombre de tu mejor amigo de la infancia?"
    };

    public EditarPerfilViewModel(AuthService authService)
    {
        _authService = authService;
        LoadUserData();
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
            ZonaHoraria = _currentUser.ZonaHoraria;
            PreguntaSeguridad = _currentUser.PreguntaSeguridad;
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
        _currentUser.ZonaHoraria = ZonaHoraria;
        
        if (!string.IsNullOrWhiteSpace(PreguntaSeguridad))
            _currentUser.PreguntaSeguridad = PreguntaSeguridad;
            
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
