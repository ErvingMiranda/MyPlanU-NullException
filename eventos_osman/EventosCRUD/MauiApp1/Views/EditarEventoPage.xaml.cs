using MauiApp1.Services;
using MauiApp1.Modelos; // Asegurate que tu clase Evento esté en esta carpeta
using System.Collections.ObjectModel;

namespace MauiApp1.Views
{
    public partial class EditarEventoPage : ContentPage
    {
        private readonly EventoService _servicio;
        public ObservableCollection<Evento> Eventos { get; set; }

        public Command<Evento> EditarCommand { get; }
        public Command<Evento> EliminarCommand { get; }

        public EditarEventoPage()
        {
            InitializeComponent();
            _servicio = EventoService.Instancia;


            // Cargar eventos en colección observable
            Eventos = new ObservableCollection<Evento>(_servicio.ObtenerEventos());

            // Usar FindByName para evitar ambigüedad con miembros generados por XAML
            var collection = this.FindByName<CollectionView>("eventosCollectionView");
            if (collection != null) collection.ItemsSource = Eventos;

            // Comandos para botones
            EditarCommand = new Command<Evento>(async (evento) =>
            {
                await Navigation.PushAsync(new FormularioEdicionPage(evento));
            });

            EliminarCommand = new Command<Evento>(async (evento) =>
            {
                bool confirmar = await DisplayAlert("Confirmar", $"¿Eliminar evento '{evento.Titulo}'?", "Sí", "No");
                if (confirmar)
                {
                    _servicio.EliminarEvento(evento.Id);
                    Eventos.Remove(evento);
                }
            });

            // Conectar comandos con la vista
            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Eventos.Clear();
            foreach (var evento in _servicio.ObtenerEventos())
            {
                Eventos.Add(evento);
            }
        }

    }
}