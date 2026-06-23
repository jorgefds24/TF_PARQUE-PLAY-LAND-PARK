using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Mantenimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMantenimiento { get; set; } // Clave primaria autogenerada

        [Required]
        public int IdJuego { get; set; } // Clave foránea para Juegos

        [Required]
        public DateTime FechaInicio { get; set; } // Fecha de inicio del mantenimiento

        [Required]
        public DateTime FechaFin { get; set; } // Fecha de fin del mantenimiento

        [StringLength(50)]
        public string Estado { get; set; } // Estado del mantenimiento: En progreso, Completo 

        [StringLength(255)]
        public string Observaciones { get; set; } // Observaciones del mantenimiento

        [ForeignKey("IdJuego")]
        public virtual Juegos Juego { get; set; } // Propiedad de navegación para la relación con Juegos
    }
}
