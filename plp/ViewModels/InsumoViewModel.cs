using Microsoft.AspNetCore.Mvc.Rendering;

namespace plp.ViewModels
{
    public class InsumoViewModel
    {
        public int IdContenido{ get; set; }
        public string Titulo { get; set; }
        public string TipoInsumo { get; set; }
        public decimal Monto { get; set; }
        public IFormFile? Foto { get; set; } // Para subir la imagen
    }
}

