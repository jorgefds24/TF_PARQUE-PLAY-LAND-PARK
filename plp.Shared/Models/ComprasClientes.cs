using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class ComprasClientes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCompra { get; set; }

        [Required]
        public int IdUsuario { get; set; } // Clave foránea para Usuarios

        [Required]
        public int IdTicket { get; set; } // Clave foránea para Tickets

        [Required]
        public DateTime FechaCompra { get; set; }

        [StringLength(100)]
        public string QRCode { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual Usuarios Usuario { get; set; } // Propiedad de navegación para Usuarios

        [ForeignKey("IdTicket")]
        public virtual Tickets Ticket { get; set; } // Propiedad de navegación para Tickets
    }
}
