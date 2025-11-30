using MauiApp1.Modelos;
using MauiApp1.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;



namespace MauiApp1.Views
{
    public partial class FormularioEdicionPage : ContentPage
    {
        private readonly EventoService _servicio;
        private readonly Evento _evento;

        public ObservableCollection<string> EtiquetasSeleccionadas { get; set; } = new();
        private List<string> etiquetasDisponibles = new() { "Crear nueva etiqueta...", "Urgente", "Importante", "Personal", "Trabajo" };

        public Command<string> EliminarEtiquetaCommand { get; }
        public Command<string> EditarEtiquetaCommand { get; }

        public FormularioEdicionPage(Evento evento)
        {
            InitializeComponent();
            BindingContext = this;

            _servicio = EventoService.Instancia;
            _evento = evento;

            EliminarEtiquetaCommand = new Command<string>(async (etiqueta) =>
            {
                bool confirmar = await DisplayAlert(
                    "Confirmar eliminación",
                    $"¿Deseas eliminar la etiqueta \"{etiqueta}\"?",
                    "Sí",
                    "No");

                if (confirmar && EtiquetasSeleccionadas.Contains(etiqueta))
                    EtiquetasSeleccionadas.Remove(etiqueta);
            });


            // Inicializar comandos
            
            EditarEtiquetaCommand = new Command<string>(async (etiqueta) =>
            {
                string nuevaEtiqueta = await DisplayPromptAsync("Editar etiqueta", "Modifica el nombre:", initialValue: etiqueta);

                if (!string.IsNullOrWhiteSpace(nuevaEtiqueta))
                {
                    int index = EtiquetasSeleccionadas.IndexOf(etiqueta);
                    if (index >= 0)
                        EtiquetasSeleccionadas[index] = nuevaEtiqueta;

                    if (!etiquetasDisponibles.Contains(nuevaEtiqueta))
                        etiquetasDisponibles.Add(nuevaEtiqueta);

                    etiquetasPicker.ItemsSource = null;
                    etiquetasPicker.ItemsSource = etiquetasDisponibles;
                }
            });

            // Cargar datos
            tituloEntry.Text = evento.Titulo;
            descripcionEditor.Text = evento.Descripcion;
            fechaInicioPicker.Date = evento.FechaInicio.Date;
            horaInicioPicker.Time = evento.FechaInicio.TimeOfDay;
            fechaFinPicker.Date = evento.FechaFin.Date;
            horaFinPicker.Time = evento.FechaFin.TimeOfDay;

            etiquetasPicker.ItemsSource = etiquetasDisponibles;

            foreach (var etiqueta in evento.Etiquetas)
                EtiquetasSeleccionadas.Add(etiqueta);
        }

        private async void OnGuardarCambiosClicked(object sender, EventArgs e)
        {
            var titulo = tituloEntry?.Text;
            var descripcion = descripcionEditor?.Text;

            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(descripcion) ||
                EtiquetasSeleccionadas.Count == 0)
            {
                await DisplayAlert("Error", "Todos los campos deben estar completos.", "OK");
                return;
            }

            _evento.Titulo = titulo;
            _evento.Descripcion = descripcion;
            _evento.FechaInicio = fechaInicioPicker.Date + horaInicioPicker.Time;
            _evento.FechaFin = fechaFinPicker.Date + horaFinPicker.Time;
            _evento.Etiquetas = EtiquetasSeleccionadas.ToList();

            _servicio.EditarEvento(_evento.Id, _evento.Titulo, _evento.Descripcion, _evento.FechaInicio, _evento.FechaFin, _evento.Etiquetas);

            await DisplayAlert("Éxito", "Evento actualizado correctamente.", "OK");
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