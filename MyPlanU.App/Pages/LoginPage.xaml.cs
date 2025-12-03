using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MyPlanU.App.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Use the x:Name-generated fields instead of FindByName
            string email = EmailEntry?.Text?.Trim();
            string password = PasswordEntry?.Text?.Trim();

            // VALIDACIONES
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Ingresa tu correo y contraseña.", "Aceptar");
                return;
            }

            // LOGIN FALSO PARA PRUEBAS (luego lo conectamos a tu backend)
            await Task.Delay(300); // Simulación

            // Comparación de correo sin distinguir mayúsculas
            if (string.Equals(email, "admin@admin.com", StringComparison.OrdinalIgnoreCase) && password == "1234")
            {
                await DisplayAlert("Bienvenido", "Inicio de sesión exitoso ", "Continuar");

                // Redirigir a la pantalla principal (AppShell)
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                // Mostrar el correo recibido para depuración (no mostrar la contraseña)
                await DisplayAlert("Acceso denegado", $"Correo o contraseña incorrectos. Correo recibido: '{email}'", "Reintentar");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistroPage());
        }
    }
}




