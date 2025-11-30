using MauiApp1.Modelos;

namespace MauiApp1.Services
{
    public class EventoService
    {
        private readonly List<Evento> _eventos = new();
        private int _nextId = 1;
        //instancia compartida, segun entiendo esto ayuda al refrescar pagina para editar eventos, donde la instancia es cambiada en las otras paginas
        public static EventoService Instancia { get; } = new EventoService();


        // CREATE
        public Evento CrearEvento(string titulo, string descripcion, DateTime inicio, DateTime fin, List<string> etiquetas)
        {
            var nuevo = new Evento
            {
                Id = _nextId++,
                Titulo = titulo,
                Descripcion = descripcion,
                FechaInicio = inicio,
                FechaFin = fin,
                Etiquetas = etiquetas ?? new List<string>()
            };
            _eventos.Add(nuevo);
            return nuevo;
        }

        // READ
        public List<Evento> ObtenerEventos() => _eventos;
        public Evento ObtenerEventoPorId(int id) => _eventos.FirstOrDefault(e => e.Id == id);

        // UPDATE
        public void EditarEvento(int id, string titulo, string descripcion, DateTime inicio, DateTime fin, List<string> etiquetas)
        {
            var e = _eventos.FirstOrDefault(ev => ev.Id == id);
            if (e != null)
            {
                e.Titulo = titulo;
                e.Descripcion = descripcion;
                e.FechaInicio = inicio;
                e.FechaFin = fin;
                e.Etiquetas = etiquetas;
            }
        }

        // DELETE
        public bool EliminarEvento(int id)
        {
            var evento = _eventos.FirstOrDefault(e => e.Id == id);
            if (evento == null) return false;

            _eventos.Remove(evento);
            return true;
        }
    }
}