using Microsoft.AspNetCore.Mvc.Rendering;

namespace plp.ViewModels
{
    public class TicketViewModel
    {
        public int IdTicket { get; set; } // Id del juego
        public string Nombre { get; set; } 
        public decimal Precio { get; set; }
        public string Color { get; set; } // Añade esta propiedad
    }
}