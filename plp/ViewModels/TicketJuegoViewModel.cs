using Microsoft.AspNetCore.Mvc.Rendering;
using plp.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace plp.ViewModels
{
    public class TicketJuegoViewModel
    {
        public TicketsJuegos TicketsJuegos { get; set; }
        public SelectList Juegos { get; set; }
        public SelectList Tickets { get; set; }

        [Display(Name = "Juegos")]
        [Required(ErrorMessage = "Por favor, selecciona al menos un juego.")]
        public int[] IdsJuegos { get; set; }

    }

}
