using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;
using MyPlanU.Backend.Services;
using MyPlanU.App.Pages;

namespace MyPlanU.App.ViewModels;

public partial class EventosViewModel : ObservableObject, IQueryAttributable
{
    private readonly ActividadService _actividadService;
    private readonly ActividadCompartidaService _actividadCompartidaService;
    private readonly AmistadService _amistadService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPromptyLiteClient _promptyClient;
    private readonly PromptyLauncher _promptyLauncher;
    private Usuario? _usuario;

    [ObservableProperty]
    private ObservableCollection<Actividad> actividades;

    [ObservableProperty]
    private ObservableCollection<Actividad> actividadesCompartidasConmigo;

    public EventosViewModel(ActividadService actividadService, 
                            ActividadCompartidaService actividadCompartidaService,
                            AmistadService amistadService,
                            IUsuarioRepository usuarioRepository,
                            IPromptyLiteClient promptyClient, 
                            PromptyLauncher promptyLauncher)
    {
        _actividadService = actividadService;
        _actividadCompartidaService = actividadCompartidaService;
        _amistadService = amistadService;
        _usuarioRepository = usuarioRepository;
        _promptyClient = promptyClient;
        _promptyLauncher = promptyLauncher;
        Actividades = new ObservableCollection<Actividad>();
        ActividadesCompartidasConmigo = new ObservableCollection<Actividad>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("Usuario"))
        {
            _usuario = query["Usuario"] as Usuario;
            // Ensure global user is set (useful if app state was restored or navigated directly)
            if (App.CurrentUser == null && _usuario != null)
            {
                App.CurrentUser = _usuario;
            }
        }
        // Reload always when navigating back
        _ = LoadActividadesAsync();
    }

    private async Task LoadActividadesAsync()
    {
        if (_usuario == null) return;
        
        // Mis actividades
        var list = await _actividadService.GetActividadesPorUsuarioAsync(_usuario.IdUsuario);
        Actividades.Clear();
        foreach (var item in list)
        {
            Actividades.Add(item);
        }

        // Actividades compartidas conmigo
        var compartidas = await _actividadCompartidaService.ObtenerActividadesCompartidasConmigoAsync(_usuario.IdUsuario);
        ActividadesCompartidasConmigo.Clear();
        foreach (var item in compartidas)
        {
            ActividadesCompartidasConmigo.Add(item);
        }
    }

    [RelayCommand]
    private async Task CompartirActividadAsync(Actividad? actividad)
    {
        if (actividad == null || _usuario == null) return;

        string action = await Shell.Current.DisplayActionSheet($"Gestionar '{actividad.Titulo}'", "Cancelar", null, "Compartir con amigo", "Ver compartidos / Dejar de compartir");

        if (action == "Compartir con amigo")
        {
            await CompartirConAmigo(actividad);
        }
        else if (action == "Ver compartidos / Dejar de compartir")
        {
            await DejarDeCompartirActividadAsync(actividad);
        }
    }

    private async Task CompartirConAmigo(Actividad actividad)
    {
        // 1. Obtener amigos
        var amigos = await _amistadService.ObtenerAmigosConfirmados(_usuario.IdUsuario);
        if (amigos.Count == 0)
        {
            await Shell.Current.DisplayAlert("Info", "No tienes amigos para compartir. Agrega amigos primero.", "OK");
            return;
        }

        // 2. Mostrar lista para seleccionar
        var nombresAmigos = amigos.Select(a => a.Usuario.Apodo ?? a.Usuario.Nombre).ToArray();
        string seleccion = await Shell.Current.DisplayActionSheet($"Compartir '{actividad.Titulo}' con:", "Cancelar", null, nombresAmigos);

        if (seleccion == "Cancelar" || seleccion == null) return;

        var amigoSeleccionado = amigos.FirstOrDefault(a => (a.Usuario.Apodo ?? a.Usuario.Nombre) == seleccion);
        if (amigoSeleccionado != null)
        {
            string modo = await Shell.Current.DisplayActionSheet("¿Cómo quieres compartir?", "Cancelar", null, "Copia (Independiente)", "Compartido (Colaborativo)");

            if (modo == "Copia (Independiente)")
            {
                bool success = await _actividadCompartidaService.CopiarActividadParaUsuarioAsync(actividad.IdActividad, amigoSeleccionado.Usuario.IdUsuario);
                if (success)
                    await Shell.Current.DisplayAlert("Éxito", $"Copia creada para {amigoSeleccionado.Usuario.Apodo ?? amigoSeleccionado.Usuario.Nombre}", "OK");
                else
                    await Shell.Current.DisplayAlert("Error", "No se pudo crear la copia", "OK");
            }
            else if (modo == "Compartido (Colaborativo)")
            {
                bool success = await _actividadCompartidaService.CompartirActividadAsync(actividad.IdActividad, _usuario.IdUsuario, amigoSeleccionado.Usuario.IdUsuario, "Editor");
                if (success)
                {
                    await Shell.Current.DisplayAlert("Éxito", $"Actividad compartida con {amigoSeleccionado.Usuario.Apodo ?? amigoSeleccionado.Usuario.Nombre}", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Info", "Ya estás compartiendo esta actividad con este usuario o hubo un error.", "OK");
                }
            }
        }
    }

    private async Task DejarDeCompartirActividadAsync(Actividad actividad)
    {
        // Obtener relaciones
        var relaciones = await _actividadCompartidaService.ObtenerRelacionesDeActividadAsync(actividad.IdActividad);
        if (relaciones.Count == 0)
        {
            await Shell.Current.DisplayAlert("Info", "No estás compartiendo esta actividad con nadie.", "OK");
            return;
        }

        // Obtener nombres de usuarios
        var opciones = new List<string>();
        var relacionMap = new Dictionary<string, ActividadCompartida>();

        foreach (var rel in relaciones)
        {
            var u = await _usuarioRepository.GetUsuarioAsync(rel.IdUsuarioDestino);
            if (u != null)
            {
                string nombre = u.Apodo ?? u.Nombre ?? u.Email;
                string key = $"{nombre} ({u.Email})";
                opciones.Add(key);
                relacionMap[key] = rel;
            }
        }

        string seleccion = await Shell.Current.DisplayActionSheet($"Dejar de compartir '{actividad.Titulo}' con:", "Cancelar", null, opciones.ToArray());

        if (seleccion == "Cancelar" || seleccion == null) return;

        if (relacionMap.TryGetValue(seleccion, out var relacion))
        {
            bool success = await _actividadCompartidaService.DejarDeCompartirActividadAsync(relacion.IdCompartirActividad);
            if (success)
            {
                await Shell.Current.DisplayAlert("Éxito", $"Has dejado de compartir con {seleccion}", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo realizar la acción.", "OK");
            }
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

        bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Estás seguro de que quieres borrar este evento?", "Sí", "No");
        if (!confirm) return;

        await _actividadService.DeleteActividadAsync(actividad);
        await Shell.Current.DisplayAlert("Éxito", "Evento eliminado con éxito", "OK");
        await LoadActividadesAsync();
    }

    [RelayCommand]
    private async Task OpenPromptyAsync()
    {
        // 1. Verificar si la API está disponible
        bool isHealthy = await _promptyClient.CheckHealthAsync();
        if (!isHealthy)
        {
            bool startServer = await Shell.Current.DisplayAlert("PROMPTY", 
                "El servidor de IA no está corriendo. ¿Deseas iniciarlo ahora?", 
                "Sí, iniciar", "Cancelar");
            
            if (!startServer) return;

            try
            {
                await _promptyLauncher.StartPromptyApiAsync();
                
                // Esperar a que levante (polling)
                bool started = false;
                // Intentamos durante 15 segundos
                for (int i = 0; i < 15; i++)
                {
                    await Task.Delay(1000); // Esperar 1s
                    if (await _promptyClient.CheckHealthAsync())
                    {
                        started = true;
                        break;
                    }
                }

                if (!started)
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo iniciar el servidor automáticamente. Intenta ejecutar 'start_prompty_api.bat' manualmente.", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Falló el inicio del servidor: {ex.Message}", "OK");
                return;
            }
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
            var (success, errorMsg) = await _promptyClient.SetTokenWithDetailsAsync(token);
            if (success)
            {
                await Shell.Current.DisplayAlert("Éxito", "Token verificado correctamente. PROMPTY ya está listo para usarse.", "OK");
                await Shell.Current.GoToAsync(nameof(PromptyPage));
                return;
            }
            else
            {
                bool retry = await Shell.Current.DisplayAlert("Error de Validación", 
                    $"El token no pudo ser verificado.\nDetalle: {errorMsg}", 
                    "Reintentar", "Cancelar");
                if (!retry) return;
            }
        }
    }

    [RelayCommand]
    private async Task GoToAmigosAsync()
    {
        await Shell.Current.GoToAsync(nameof(AmigosPage));
    }

    [RelayCommand]
    private async Task GoToConfigAsync()
    {
        await Shell.Current.GoToAsync(nameof(ConfiguracionPage));
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}?Logout=true");
    }

    [RelayCommand]
    private async Task ExitApp()
    {
        bool confirm = await Shell.Current.DisplayAlert("Salir", "¿Seguro que quieres salir?", "Sí", "No");
        if (confirm)
        {
            Application.Current.Quit();
        }
    }
}
