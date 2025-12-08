using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;
using System.Collections.ObjectModel;

namespace MyPlanU.App.ViewModels;

public partial class AmigosViewModel : ObservableObject
{
    private readonly AmistadService _amistadService;
    
    [ObservableProperty]
    private string emailDestino = string.Empty;

    public ObservableCollection<AmigoInfo> Amigos { get; } = new();
    public ObservableCollection<AmigoInfo> SolicitudesEntrantes { get; } = new();
    public ObservableCollection<AmigoInfo> SolicitudesSalientes { get; } = new();

    public AmigosViewModel(AmistadService amistadService)
    {
        _amistadService = amistadService;
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
            await Shell.Current.DisplayAlert("Error", "No se pudo enviar la solicitud (Usuario no existe, ya son amigos o solicitud pendiente)", "OK");
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
