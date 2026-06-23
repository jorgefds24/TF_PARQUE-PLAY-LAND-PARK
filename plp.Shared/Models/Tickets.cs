using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Tickets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTicket { get; set; } // Clave primaria autogenerada

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } // Nombre del ticket: Blanco, Celeste, Verde

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; } // Precio del ticket    
        public string Color { get; set; }

        public virtual ICollection<ComprasClientes> ComprasClientes { get; set; } // Relación uno a muchos con ComprasClientes

        public virtual ICollection<TicketsJuegos> TicketsJuegos { get; set; } // Relación muchos a muchos con Juegos

        public Tickets()
        {
            ComprasClientes = new HashSet<ComprasClientes>();
            TicketsJuegos = new HashSet<TicketsJuegos>();
        }
    }
}

