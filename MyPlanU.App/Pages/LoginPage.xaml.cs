using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MyPlanU.Backend.Business;

namespace MyPlanU.App.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly AuthService _authService;

        public LoginPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
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

            // LOGIN REAL CON AUTHSERVICE
            var usuario = await _authService.LoginAsync(email, password);

            if (usuario != null)
            {
                await DisplayAlert("Bienvenido", "Inicio de sesión exitoso ", "Continuar");

                var navigationParameter = new Dictionary<string, object>
                {
                    { "Usuario", usuario }
                };
                await Shell.Current.GoToAsync(nameof(EventosPage), navigationParameter);
            }
            else
            {
                // Mensaje corregido sin mostrar el correo recibido
                await DisplayAlert("Acceso denegado", "Correo o contraseña incorrectos.", "Reintentar");
            }
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegistroPage));
        }
    }
}




