using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.ViewModels;

public partial class CrearEventoViewModel : ObservableObject, IQueryAttributable
{
    private readonly ActividadService _actividadService;
    private Usuario? _usuario;
    private Actividad? _actividadExistente;

    [ObservableProperty]
    private string titulo = string.Empty;

    [ObservableProperty]
    private string descripcion = string.Empty;

    [ObservableProperty]
    private string pageTitle = "Crear Evento";

    [ObservableProperty]
    private string headerTitle = "Nuevo Evento";

    [ObservableProperty]
    private ObservableCollection<string> etiquetasSeleccionadas = new();

    [ObservableProperty]
    private ObservableCollection<string> etiquetasDisponibles = new()
    {
        "Trabajo", "Personal", "Urgente", "Importante", "Crear nueva..."
    };

    [ObservableProperty]
    private string etiquetaSeleccionadaPicker;

    [ObservableProperty]
    private bool isNuevaEtiquetaVisible;

    [ObservableProperty]
    private string nuevaEtiquetaTexto = string.Empty;

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

    partial void OnEtiquetaSeleccionadaPickerChanged(string value)
    {
        if (value == "Crear nueva...")
        {
            IsNuevaEtiquetaVisible = true;
        }
        else if (!string.IsNullOrEmpty(value))
        {
            IsNuevaEtiquetaVisible = false;
            if (!EtiquetasSeleccionadas.Contains(value))
            {
                EtiquetasSeleccionadas.Add(value);
            }
            // Reset picker to allow re-selecting same item if removed
            EtiquetaSeleccionadaPicker = null; 
        }
    }

    [RelayCommand]
    private void AgregarNuevaEtiqueta()
    {
        if (!string.IsNullOrWhiteSpace(NuevaEtiquetaTexto))
        {
            // Insert before "Crear nueva..."
            if (!EtiquetasDisponibles.Contains(NuevaEtiquetaTexto))
            {
                EtiquetasDisponibles.Insert(EtiquetasDisponibles.Count - 1, NuevaEtiquetaTexto);
            }
            
            if (!EtiquetasSeleccionadas.Contains(NuevaEtiquetaTexto))
            {
                EtiquetasSeleccionadas.Add(NuevaEtiquetaTexto);
            }

            NuevaEtiquetaTexto = string.Empty;
            IsNuevaEtiquetaVisible = false;
        }
    }

    [RelayCommand]
    private void EliminarEtiqueta(string etiqueta)
    {
        if (EtiquetasSeleccionadas.Contains(etiqueta))
        {
            EtiquetasSeleccionadas.Remove(etiqueta);
        }
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
                PageTitle = "Editar Evento";
                HeaderTitle = "Editar Evento";
                Titulo = _actividadExistente.Titulo;
                Descripcion = _actividadExistente.Descripcion;
                
                EtiquetasSeleccionadas.Clear();
                if (!string.IsNullOrEmpty(_actividadExistente.Etiquetas))
                {
                    var tags = _actividadExistente.Etiquetas.Split('|', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var tag in tags)
                    {
                        EtiquetasSeleccionadas.Add(tag);
                        if (!EtiquetasDisponibles.Contains(tag))
                        {
                            EtiquetasDisponibles.Insert(EtiquetasDisponibles.Count - 1, tag);
                        }
                    }
                }

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

        var etiquetasString = string.Join("|", EtiquetasSeleccionadas);

        if (_actividadExistente != null)
        {
            // Editar existente
            _actividadExistente.Titulo = Titulo;
            _actividadExistente.Descripcion = Descripcion;
            _actividadExistente.Etiquetas = etiquetasString;
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
                Etiquetas = etiquetasString,
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
