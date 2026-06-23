using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace plp.Shared.Models
{
    public class TicketsJuegos
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdJuego { get; set; } // Clave foránea para Juegos

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTicket { get; set; } // Clave foránea para Tickets 

        [ForeignKey("IdJuego")]
        public virtual Juegos Juego { get; set; }

        [ForeignKey("IdTicket")]
        public virtual Tickets Ticket { get; set; }
    }
}
