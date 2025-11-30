using MauiApp1.Services;
using MauiApp1.Modelos;

using System.Collections.ObjectModel;

namespace MauiApp1.Views
{
    public partial class EventosPage : ContentPage
    {
        public ObservableCollection<Evento> Eventos { get; set; } = new();
        private readonly EventoService _servicio;

        public EventosPage()
        {
            InitializeComponent();
            BindingContext = this;
            _servicio = EventoService.Instancia;

            CargarEventos();
        }

        private void CargarEventos()
        {
            Eventos.Clear();
            foreach (var evento in _servicio.ObtenerEventos())
                Eventos.Add(evento);
        }

        private async void OnAgregarEventoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CrearEventoPage(OnEventoCreado));
        }

        private async void OnEditarEventoClicked(object sender, EventArgs e)
        {
            if (sender is Button boton && boton.CommandParameter is Evento evento)
                await Navigation.PushAsync(new FormularioEdicionPage(evento));
        }

        private async void OnEliminarEventoClicked(object sender, EventArgs e)
        {
            if (sender is Button boton && boton.CommandParameter is Evento evento)
            {
                bool confirmar = await DisplayAlert("Confirmar", $"¿Eliminar el evento \"{evento.Titulo}\"?", "Sí", "No");
                if (confirmar)
                {
                    _servicio.EliminarEvento(evento.Id);
                    CargarEventos();
                }
            }
        }

        private void OnEventoCreado(Evento nuevoEvento)
        {
            Eventos.Add(nuevoEvento);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CargarEventos(); // Refresca la lista al volver desde edición
        }
    }
}