using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MyPlanU.App.ViewModels;

public class RegistroViewModel
{
    // PROPIEDADES DEL FORMULARIO
    public string Nombre { get; set; }
    public string Email  { get; set; }
    public string Password { get; set; }

    // COMMAND PARA EL BOTÓN REGISTRARSE
    public ICommand RegistrarCommand { get; }
    public ICommand GoBackCommand { get; }

    public RegistroViewModel()
    {
        // Command de MAUI (nativo, sin librerías extra)
        RegistrarCommand = new Command(async () => await Registrar());
        GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
    }

    private async Task Registrar()
    {
        // VALIDACIONES BÁSICAS
        if (string.IsNullOrWhiteSpace(Nombre) ||
            string.IsNullOrWhiteSpace(Email)  ||
            string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "Por favor completa todos los campos.",
                "Aceptar");
            return;
        }

        // AQUÍ IRÍA TU LÓGICA REAL DE REGISTRO (BD o API)
        await Task.Delay(500); // Simulación

        await Application.Current.MainPage.DisplayAlert(
            "¡Registrado!",
            "Tu cuenta fue creada correctamente 💜",
            "Continuar");

        // Navegar atrás (volver al Login)
        await Application.Current.MainPage.Navigation.PopAsync();
    }
}


