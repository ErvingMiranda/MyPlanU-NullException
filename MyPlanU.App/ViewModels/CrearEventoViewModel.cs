using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public partial class CrearEventoViewModel : ObservableObject
{
    private readonly ActividadService _actividadService;
    private Usuario _usuario;

    [ObservableProperty]
    private string titulo;

    [ObservableProperty]
    private string descripcion;

    [ObservableProperty]
    private DateTime fechaInicio = DateTime.Now;

    [ObservableProperty]
    private TimeSpan horaInicio = DateTime.Now.TimeOfDay;

    [ObservableProperty]
    private DateTime fechaFin = DateTime.Now.AddHours(1);

    [ObservableProperty]
    private TimeSpan horaFin = DateTime.Now.AddHours(1).TimeOfDay;

    public CrearEventoViewModel(ActividadService actividadService)
    {
        _actividadService = actividadService;
    }

    public void SetUsuario(Usuario usuario)
    {
        _usuario = usuario;
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(Descripcion))
        {
            await Shell.Current.DisplayAlert("Error", "Complete título y descripción", "OK");
            return;
        }

        var inicio = FechaInicio.Date + HoraInicio;
        var fin = FechaFin.Date + HoraFin;

        if (fin <= inicio)
        {
            await Shell.Current.DisplayAlert("Error", "La fecha de fin debe ser posterior al inicio", "OK");
            return;
        }

        var nuevaActividad = new Actividad
        {
            IdUsuarioCreador = _usuario?.IdUsuario ?? 0, // Should handle null user better
            Titulo = Titulo,
            Descripcion = Descripcion,
            FechaInicio = inicio,
            FechaFin = fin,
            FechaCreacion = DateTime.Now,
            Estado = "Pendiente",
            Prioridad = "Normal"
        };

        await _actividadService.SaveActividadAsync(nuevaActividad);
        await Shell.Current.GoToAsync("..");
    }
}
