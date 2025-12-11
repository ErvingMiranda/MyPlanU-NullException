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
        "Trabajo", "Personal", "Urgente", "Importante", "Crear nueva...", "Eliminar..."
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
            EtiquetaSeleccionadaPicker = null;
        }
        else if (value == "Eliminar...")
        {
            EliminarEtiquetaDisponibleAsync();
            EtiquetaSeleccionadaPicker = null;
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

    private async void EliminarEtiquetaDisponibleAsync()
    {
        var tagsToDelete = EtiquetasDisponibles
            .Where(t => t != "Crear nueva..." && t != "Eliminar...")
            .ToArray();

        if (tagsToDelete.Length == 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "No hay etiquetas para eliminar.", "OK");
            return;
        }

        string result = await Shell.Current.DisplayActionSheet("Eliminar etiqueta", "Cancelar", null, tagsToDelete);

        if (result != "Cancelar" && !string.IsNullOrEmpty(result))
        {
            EtiquetasDisponibles.Remove(result);
            if (EtiquetasSeleccionadas.Contains(result))
            {
                EtiquetasSeleccionadas.Remove(result);
            }
        }
    }

    [RelayCommand]
    private void AgregarNuevaEtiqueta()
    {
        if (!string.IsNullOrWhiteSpace(NuevaEtiquetaTexto))
        {
            // Insert before "Crear nueva..."
            // "Crear nueva..." is at Count - 2 now because of "Eliminar..."
            // Or just find the index of "Crear nueva..."
            int index = EtiquetasDisponibles.IndexOf("Crear nueva...");
            if (index == -1) index = EtiquetasDisponibles.Count;
            
            if (!EtiquetasDisponibles.Contains(NuevaEtiquetaTexto))
            {
                EtiquetasDisponibles.Insert(index, NuevaEtiquetaTexto);
            }
            
            if (!EtiquetasSeleccionadas.Contains(NuevaEtiquetaTexto))
            {
                EtiquetasSeleccionadas.Add(NuevaEtiquetaTexto);
            }

            NuevaEtiquetaTexto = string.Empty;
            IsNuevaEtiquetaVisible = false;
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
                            // Insert before "Crear nueva..."
                            int index = EtiquetasDisponibles.IndexOf("Crear nueva...");
                            if (index == -1) index = EtiquetasDisponibles.Count;
                            EtiquetasDisponibles.Insert(index, tag);
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
        if (string.IsNullOrWhiteSpace(Titulo))
        {
            await Shell.Current.DisplayAlert("Error", "El título es obligatorio", "OK");
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
            await Shell.Current.DisplayAlert("Éxito", "Evento actualizado con éxito", "OK");
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
            await Shell.Current.DisplayAlert("Éxito", "Evento creado con éxito", "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}
