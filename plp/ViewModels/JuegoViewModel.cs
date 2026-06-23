using Microsoft.AspNetCore.Mvc.Rendering;

namespace plp.ViewModels
{
    public class JuegoViewModel
    {
        public int IdJuego { get; set; } // Id del juego
        public string Nombre { get; set; } // Nombre del juego
        public string Estado { get; set; } // Estado del juego: Libre, Ocupado, etc.
        public int Capacidad { get; set; } // Capacidad o aforo del juego
        public IFormFile? Foto { get; set; } // Para subir la imagen
    }
}