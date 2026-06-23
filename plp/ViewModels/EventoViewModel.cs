using Microsoft.AspNetCore.Mvc.Rendering;


namespace plp.ViewModels
{
    public class EventoViewModel
    {
        public int IdContenido { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaEvento { get; set; }
        public DateTime HoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public IFormFile? Foto { get; set; } // Para subir la imagen
    }
}
