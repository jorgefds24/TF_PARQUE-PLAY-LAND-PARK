using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Juegos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdJuego { get; set; } // Clave primaria autogenerada

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } // Nombre del juego

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } // Estado del juego: Libre, Ocupado, etc.

        [Required]
        public int Capacidad { get; set; } // Capacidad o aforo del juego

        public string? Imagen { get; set; } // Nombre de la imagen

        public virtual ICollection<TicketsJuegos> TicketsJuegos { get; set; } // Relación muchos a muchos con Tickets

        public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } // Relación uno a muchos con Mantenimiento

        public virtual ICollection<Comentarios> Comentarios { get; set; } // Relación uno a muchos con Comentarios

        public virtual HorariosJuegos HorarioJuego { get; set; } // Relación uno a uno con HorariosJuegos

        // Para inicializar y crear una nueva instancia de la coleccion
        public Juegos()
        {
            TicketsJuegos = new HashSet<TicketsJuegos>();
            Mantenimientos = new HashSet<Mantenimiento>();
            Comentarios = new HashSet<Comentarios>();
        }
    }
}
