using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Business;

namespace MyPlanU.App.ViewModels;

public partial class RecuperarPasswordViewModel : ObservableObject
{
    private readonly AuthService _authService;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string securityQuestion;

    [ObservableProperty]
    private string securityAnswer;

    [ObservableProperty]
    private string newPassword;

    [ObservableProperty]
    private bool isNewPasswordHidden = true;

    [RelayCommand]
    private void ToggleNewPasswordVisibility() => IsNewPasswordHidden = !IsNewPasswordHidden;

    [ObservableProperty]
    private bool isQuestionVisible;

    [ObservableProperty]
    private bool isPasswordVisible;

    public RecuperarPasswordViewModel(AuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task FindUserAsync()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlert("Error", "Ingrese su correo", "OK");
            return;
        }

        var question = await _authService.GetSecurityQuestionAsync(Email);
        if (string.IsNullOrEmpty(question))
        {
            await Shell.Current.DisplayAlert("Error", "Usuario no encontrado o sin pregunta de seguridad", "OK");
            return;
        }

        SecurityQuestion = question;
        IsQuestionVisible = true;
    }

    [RelayCommand]
    private async Task VerifyAnswerAsync()
    {
        if (string.IsNullOrWhiteSpace(SecurityAnswer))
        {
            await Shell.Current.DisplayAlert("Error", "Ingrese la respuesta", "OK");
            return;
        }

        bool isValid = await _authService.VerifySecurityAnswerAsync(Email, SecurityAnswer);
        if (isValid)
        {
            IsPasswordVisible = true;
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Respuesta incorrecta", "OK");
        }
    }

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            await Shell.Current.DisplayAlert("Error", "Ingrese la nueva contraseña", "OK");
            return;
        }

        string errorPass = ValidationHelper.ValidarContrasena(NewPassword);
        if (!string.IsNullOrEmpty(errorPass))
        {
            await Shell.Current.DisplayAlert("Error", errorPass, "OK");
            return;
        }

        bool success = await _authService.ResetPasswordAsync(Email, NewPassword);
        if (success)
        {
            await Shell.Current.DisplayAlert("Éxito", "Contraseña restablecida", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "No se pudo restablecer la contraseña", "OK");
        }
    }
    
    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
