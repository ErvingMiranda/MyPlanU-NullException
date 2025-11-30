using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;
using MyPlanU.App.Pages;

namespace MyPlanU.App.ViewModels;

public partial class EventosViewModel : ObservableObject, IQueryAttributable
{
    private readonly ActividadService _actividadService;
    private Usuario _usuario;

    [ObservableProperty]
    private ObservableCollection<Actividad> actividades;

    public EventosViewModel(ActividadService actividadService)
    {
        _actividadService = actividadService;
        Actividades = new ObservableCollection<Actividad>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Usuario"))
        {
            _usuario = query["Usuario"] as Usuario;
            LoadActividadesAsync();
        }
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
        // Logic to add event (maybe navigate to a Create page or show a popup)
        // For now, just add a dummy one
        var nueva = new Actividad
        {
            IdUsuarioCreador = _usuario.IdUsuario,
            Titulo = "Nueva Actividad",
            Descripcion = "Descripción de prueba",
            FechaInicio = DateTime.Now,
            FechaFin = DateTime.Now.AddHours(1),
            FechaCreacion = DateTime.Now
        };
        await _actividadService.SaveActividadAsync(nueva);
        await LoadActividadesAsync();
    }

    [RelayCommand]
    private async Task EditarEventoAsync(Actividad actividad)
    {
        // Logic to edit
        await Shell.Current.DisplayAlert("Editar", $"Editar {actividad.Titulo}", "OK");
    }

    [RelayCommand]
    private async Task EliminarEventoAsync(Actividad actividad)
    {
        await _actividadService.DeleteActividadAsync(actividad);
        await LoadActividadesAsync();
    }

    [RelayCommand]
    private async Task OpenPromptyAsync()
    {
        // Stub
        await Shell.Current.DisplayAlert("PROMPTY", "Abriendo Prompty...", "OK");
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
