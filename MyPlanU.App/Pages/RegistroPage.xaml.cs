using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MyPlanU.App.Pages
{
    public partial class RegistroPage : ContentPage
    {
        public RegistroPage()
        {
            InitializeComponent();   // 👈 SOLO se llama, NO se define
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

            await Task.Delay(500); // simulación

            await DisplayAlert(
                "Registrado",
                "Tu cuenta fue creada correctamente 💜",
                "Continuar");

            await Navigation.PopAsync();
        }
    }
}


