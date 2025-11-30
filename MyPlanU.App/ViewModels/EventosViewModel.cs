using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;
using MyPlanU.Backend.Services;
using MyPlanU.App.Pages;

namespace MyPlanU.App.ViewModels;

public partial class EventosViewModel : ObservableObject, IQueryAttributable
{
    private readonly ActividadService _actividadService;
    private readonly IPromptyLiteClient _promptyClient;
    private Usuario? _usuario;

    [ObservableProperty]
    private ObservableCollection<Actividad> actividades;

    public EventosViewModel(ActividadService actividadService, IPromptyLiteClient promptyClient)
    {
        _actividadService = actividadService;
        _promptyClient = promptyClient;
        Actividades = new ObservableCollection<Actividad>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Usuario"))
        {
            _usuario = query["Usuario"] as Usuario;
        }
        // Reload always when navigating back
        _ = LoadActividadesAsync();
    }

    private async Task LoadActividadesAsync()
    {
        if (_usuario == null) return;
        var list = await _actividadService.GetActividadesPorUsuarioAsync(_usuario.IdUsuario);
        Actividades.Clear();
        foreach (var item in list)
        {
            Actividades.Add(item);
        }
    }

    [RelayCommand]
    private async Task AgregarEventoAsync()
    {
        if (_usuario == null) return;
        var navigationParameter = new Dictionary<string, object>
        {
            { "Usuario", _usuario }
        };
        await Shell.Current.GoToAsync(nameof(CrearEventoPage), navigationParameter);
    }

    [RelayCommand]
    private async Task EditarEventoAsync(Actividad? actividad)
    {
        if (actividad == null || _usuario == null) return;
        
        var navigationParameter = new Dictionary<string, object>
        {
            { "Usuario", _usuario },
            { "Actividad", actividad }
        };
        await Shell.Current.GoToAsync(nameof(CrearEventoPage), navigationParameter);
    }

    [RelayCommand]
    private async Task EliminarEventoAsync(Actividad? actividad)
    {
        if (actividad == null) return;
        await _actividadService.DeleteActividadAsync(actividad);
        await LoadActividadesAsync();
    }

    [RelayCommand]
    private async Task OpenPromptyAsync()
    {
        // 1. Verificar si la API está disponible
        bool isHealthy = await _promptyClient.CheckHealthAsync();
        if (!isHealthy)
        {
            await Shell.Current.DisplayAlert("Error", "No se puede conectar con PROMPTY. Asegúrate de que el servidor Python esté corriendo.", "OK");
            return;
        }

        // 2. Verificar si ya hay un token válido
        bool hasToken = await _promptyClient.HasValidTokenAsync();
        if (hasToken)
        {
            await Shell.Current.GoToAsync(nameof(PromptyPage));
            return;
        }

        // 3. Pedir token al usuario
        while (true)
        {
            string token = await Shell.Current.DisplayPromptAsync("Configuración PROMPTY", 
                "Para usar el modo inteligente, ingresa tu token de Hugging Face (gratuito). Se guardará localmente.", 
                "Validar", "Cancelar", "hf_...");
            
            if (string.IsNullOrWhiteSpace(token))
            {
                // Usuario canceló o ingresó vacío
                return;
            }

            // 4. Validar y guardar token
            bool success = await _promptyClient.SetTokenAsync(token);
            if (success)
            {
                await Shell.Current.DisplayAlert("Éxito", "Token verificado correctamente. PROMPTY ya está listo para usarse.", "OK");
                await Shell.Current.GoToAsync(nameof(PromptyPage));
                return;
            }
            else
            {
                bool retry = await Shell.Current.DisplayAlert("Error", "El token no es válido o hubo un error al verificarlo.", "Reintentar", "Cancelar");
                if (!retry) return;
            }
        }
    }

    [RelayCommand]
    private async Task GoToConfigAsync()
    {
        await Shell.Current.GoToAsync(nameof(ConfiguracionPage));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    [RelayCommand]
    private void ExitApp()
    {
        Application.Current.Quit();
    }
}
