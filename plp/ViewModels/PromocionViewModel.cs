using Microsoft.AspNetCore.Mvc.Rendering;

namespace plp.ViewModels
{
    public class PromocionViewModel
    {
        public int IdContenido { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Descuento { get; set; }
        public IFormFile? Foto { get; set; } // Para subir la imagen
    }
}
