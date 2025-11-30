using System.Collections.ObjectModel;
using MauiApp1.Modelos;
using MauiApp1.Services;

namespace MauiApp1.Views
{
    public partial class CrearEventoPage : ContentPage
    {
        private readonly EventoService _servicio;
        private readonly Action<Evento> _onEventoCreado;

        public ObservableCollection<string> EtiquetasSeleccionadas { get; set; } = new();
        private List<string> etiquetasDisponibles = new() { "Crear nueva etiqueta...", "Urgente", "Importante", "Personal", "Trabajo" };

        public Command<string> EliminarEtiquetaCommand { get; }

        public CrearEventoPage(Action<Evento> onEventoCreado)
        {
            InitializeComponent();
            BindingContext = this;

            _servicio = EventoService.Instancia;
            _onEventoCreado = onEventoCreado;

            etiquetasPicker.ItemsSource = etiquetasDisponibles;

            EliminarEtiquetaCommand = new Command<string>((etiqueta) =>
            {
                if (EtiquetasSeleccionadas.Contains(etiqueta))
                    EtiquetasSeleccionadas.Remove(etiqueta);
            });
        }

        private async void OnCrearClicked(object sender, EventArgs e)
        {
            var titulo = tituloEntry.Text?.Trim();
            var descripcion = descripcionEditor.Text?.Trim();
            var fechaInicio = fechaInicioPicker.Date + horaInicioPicker.Time;
            var fechaFin = fechaFinPicker.Date + horaFinPicker.Time;

            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(descripcion) ||
                EtiquetasSeleccionadas.Count == 0)
            {
                await DisplayAlert("Error", "Todos los campos deben estar completos y al menos una etiqueta seleccionada.", "OK");
                return;
            }

            var eventoCreado = _servicio.CrearEvento(
                titulo,
                descripcion,
                fechaInicio,
                fechaFin,
                EtiquetasSeleccionadas.ToList());

            _onEventoCreado?.Invoke(eventoCreado);

            await DisplayAlert("Éxito", "Evento creado correctamente.", "OK");
            await Navigation.PopAsync();
        }

        private void OnEtiquetaSeleccionada(object sender, EventArgs e)
        {
            var seleccionada = etiquetasPicker.SelectedItem?.ToString();

            crearEtiquetaPanel.IsVisible = seleccionada == "Crear nueva etiqueta...";

            if (!string.IsNullOrWhiteSpace(seleccionada) &&
                seleccionada != "Crear nueva etiqueta..." &&
                !EtiquetasSeleccionadas.Contains(seleccionada))
            {
                EtiquetasSeleccionadas.Add(seleccionada);
            }
        }

        private void OnAgregarEtiquetaClicked(object sender, EventArgs e)
        {
            var nueva = nuevaEtiquetaEntry.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(nueva))
            {
                if (!etiquetasDisponibles.Contains(nueva))
                    etiquetasDisponibles.Add(nueva);

                if (!EtiquetasSeleccionadas.Contains(nueva))
                    EtiquetasSeleccionadas.Add(nueva);

                nuevaEtiquetaEntry.Text = string.Empty;
                etiquetasPicker.ItemsSource = null;
                etiquetasPicker.ItemsSource = etiquetasDisponibles;
            }
        }
    }
}