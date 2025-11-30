using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MauiApp1.Modelos
{
    public class Evento : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string? titulo;
        public string? Titulo
        {
            get => titulo;
            set
            {
                if (titulo != value)
                {
                    titulo = value;
                    OnPropertyChanged(nameof(Titulo));
                }
            }
        }

        private string? descripcion;
        public string? Descripcion
        {
            get => descripcion;
            set
            {
                if (descripcion != value)
                {
                    descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
                }
            }
        }

        private DateTime fechaInicio;
        public DateTime FechaInicio
        {
            get => fechaInicio;
            set
            {
                if (fechaInicio != value)
                {
                    fechaInicio = value;
                    OnPropertyChanged(nameof(FechaInicio));
                }
            }
        }

        private DateTime fechaFin;
        public DateTime FechaFin
        {
            get => fechaFin;
            set
            {
                if (fechaFin != value)
                {
                    fechaFin = value;
                    OnPropertyChanged(nameof(FechaFin));
                }
            }
        }

        private List<string> etiquetas = new();
        public List<string> Etiquetas
        {
            get => etiquetas;
            set
            {
                if (etiquetas != value)
                {
                    etiquetas = value;
                    OnPropertyChanged(nameof(Etiquetas));
                }
            }
        }

        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}