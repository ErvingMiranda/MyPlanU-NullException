using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;

namespace MyPlanU.App.ViewModels;

public partial class CambiarPasswordViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string currentPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    public CambiarPasswordViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task CambiarAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword) || 
            string.IsNullOrWhiteSpace(NewPassword) || 
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas nuevas no coinciden", "OK");
            return;
        }

        if (App.CurrentUser == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "No hay sesión activa", "OK");
            return;
        }

        bool success = await _authService.ChangePasswordAsync(App.CurrentUser.IdUsuario, CurrentPassword, NewPassword);

        if (success)
        {
            await Application.Current.MainPage.DisplayAlert("Éxito", "Contraseña actualizada correctamente", "OK");
            await Shell.Current.GoToAsync(".."); // Volver atrás
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Error", "La contraseña actual es incorrecta", "OK");
        }
    }
}
