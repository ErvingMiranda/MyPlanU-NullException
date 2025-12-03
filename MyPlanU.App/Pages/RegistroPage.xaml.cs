using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Models;

namespace MyPlanU.App.Pages
{
    public partial class RegistroPage : ContentPage
    {
        private readonly AuthService _authService;

        public RegistroPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnRegistrarseClicked(object sender, EventArgs e)
        {
            var nombreEntry = this.FindByName<Entry>("NombreEntry");
            var emailEntry = this.FindByName<Entry>("EmailEntry");
            var passwordEntry = this.FindByName<Entry>("PasswordEntry");

            string nombre = nombreEntry?.Text?.Trim();
            string email = emailEntry?.Text?.Trim();
            string password = passwordEntry?.Text?.Trim();

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert(
                    "Error",
                    "Por favor completa todos los campos.",
                    "Aceptar");
                return;
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Email = email,
                ContrasenaHash = password,
                FechaRegistro = DateTime.Now
            };

            bool exito = await _authService.RegisterAsync(nuevoUsuario);

            if (exito)
            {
                await DisplayAlert(
                    "Registrado",
                    "Tu cuenta fue creada correctamente 💜",
                    "Continuar");

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await DisplayAlert("Error", "El correo ya está registrado.", "Aceptar");
            }
        }
    }
}


