using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;
using System.Collections.ObjectModel;

namespace MyPlanU.App.ViewModels;

public partial class AmigosViewModel : ObservableObject
{
    private readonly AmistadService _amistadService;
    private readonly UserService _userService;
    
    [ObservableProperty]
    private string emailDestino = string.Empty;

    [ObservableProperty]
    private string searchText = string.Empty;

    public ObservableCollection<AmigoInfo> Amigos { get; } = new();
    public ObservableCollection<AmigoInfo> SolicitudesEntrantes { get; } = new();
    public ObservableCollection<AmigoInfo> SolicitudesSalientes { get; } = new();
    public ObservableCollection<Usuario> SearchResults { get; } = new();

    public AmigosViewModel(AmistadService amistadService, UserService userService)
    {
        _amistadService = amistadService;
        _userService = userService;
    }

    [RelayCommand]
    public async Task SearchUsuariosAsync()
    {
        if (App.CurrentUser == null) return;
        SearchResults.Clear();
        if (string.IsNullOrWhiteSpace(SearchText)) return;

        var results = await _userService.SearchUsuariosAsync(SearchText, App.CurrentUser.IdUsuario);
        foreach (var u in results)
        {
            SearchResults.Add(u);
        }
    }

    [RelayCommand]
    public async Task EnviarSolicitudDesdeBusquedaAsync(Usuario usuario)
    {
        if (usuario == null) return;
        if (App.CurrentUser == null) return;

        bool success = await _amistadService.EnviarSolicitud(App.CurrentUser.IdUsuario, usuario.Email);
        if (success)
        {
            await Shell.Current.DisplayAlert("Éxito", "Solicitud enviada con éxito", "OK");
            await LoadDataAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Info", "No se pudo enviar (Ya son amigos o solicitud pendiente)", "OK");
        }
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (App.CurrentUser == null) return;

        Amigos.Clear();
        SolicitudesEntrantes.Clear();
        SolicitudesSalientes.Clear();

        var amigos = await _amistadService.ObtenerAmigosConfirmados(App.CurrentUser.IdUsuario);
        foreach (var a in amigos) Amigos.Add(a);

        var entrantes = await _amistadService.ObtenerSolicitudesEntrantes(App.CurrentUser.IdUsuario);
        foreach (var e in entrantes) SolicitudesEntrantes.Add(e);

        var salientes = await _amistadService.ObtenerSolicitudesSalientes(App.CurrentUser.IdUsuario);
        foreach (var s in salientes) SolicitudesSalientes.Add(s);
    }

    [RelayCommand]
    public async Task EnviarSolicitudAsync()
    {
        if (string.IsNullOrWhiteSpace(EmailDestino))
        {
            await Shell.Current.DisplayAlert("Error", "Ingrese un email", "OK");
            return;
        }

        if (App.CurrentUser == null) return;

        bool success = await _amistadService.EnviarSolicitud(App.CurrentUser.IdUsuario, EmailDestino);
        if (success)
        {
            await Shell.Current.DisplayAlert("Éxito", "Solicitud enviada", "OK");
            EmailDestino = string.Empty;
            await LoadDataAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Info", "No se pudo enviar (Ya son amigos o solicitud pendiente)", "OK");
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }
}

    [RelayCommand]
    public async Task AceptarSolicitudAsync(AmigoInfo info)
    {
        if (info == null) return;
        await _amistadService.AceptarSolicitud(info.IdAmistad);
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task RechazarSolicitudAsync(AmigoInfo info)
    {
        if (info == null) return;
        await _amistadService.RechazarSolicitud(info.IdAmistad);
        await LoadDataAsync();
    }

    [RelayCommand]
    public async Task EliminarAmigoAsync(AmigoInfo info)
    {
        if (info == null) return;
        bool confirm = await Shell.Current.DisplayAlert("Confirmar", $"¿Eliminar a {info.Usuario.Nombre}?", "Sí", "No");
        if (confirm)
        {
            await _amistadService.EliminarAmigo(info.IdAmistad);
            await LoadDataAsync();
        }
    }
}
