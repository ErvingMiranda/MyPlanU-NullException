using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public partial class CrearEventoViewModel : ObservableObject, IQueryAttributable
{
    private readonly ActividadService _actividadService;
    private Usuario _usuario;
    private Actividad _actividadExistente;

    [ObservableProperty]
    private string titulo;

    [ObservableProperty]
    private string descripcion;

    [ObservableProperty]
    private string etiqueta;

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

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Usuario"))
        {
            _usuario = query["Usuario"] as Usuario;
        }

        if (query.ContainsKey("Actividad"))
        {
            _actividadExistente = query["Actividad"] as Actividad;
            if (_actividadExistente != null)
            {
                Titulo = _actividadExistente.Titulo;
                Descripcion = _actividadExistente.Descripcion;
                Etiqueta = _actividadExistente.Etiqueta;
                FechaInicio = _actividadExistente.FechaInicio.Date;
                HoraInicio = _actividadExistente.FechaInicio.TimeOfDay;
                FechaFin = _actividadExistente.FechaFin.Date;
                HoraFin = _actividadExistente.FechaFin.TimeOfDay;
            }
        }
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

        if (_actividadExistente != null)
        {
            // Editar existente
            _actividadExistente.Titulo = Titulo;
            _actividadExistente.Descripcion = Descripcion;
            _actividadExistente.Etiqueta = Etiqueta;
            _actividadExistente.FechaInicio = inicio;
            _actividadExistente.FechaFin = fin;
            
            await _actividadService.SaveActividadAsync(_actividadExistente);
        }
        else
        {
            // Crear nueva
            var nuevaActividad = new Actividad
            {
                IdUsuarioCreador = _usuario?.IdUsuario ?? 0,
                Titulo = Titulo,
                Descripcion = Descripcion,
                Etiqueta = Etiqueta,
                FechaInicio = inicio,
                FechaFin = fin,
                FechaCreacion = DateTime.Now,
                Estado = "Pendiente",
                Prioridad = "Normal"
            };
            await _actividadService.SaveActividadAsync(nuevaActividad);
        }

        await Shell.Current.GoToAsync("..");
    }
}
