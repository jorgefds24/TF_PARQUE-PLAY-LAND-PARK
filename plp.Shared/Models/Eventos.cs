using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Eventos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEvento { get; set; } // Clave primaria de Eventos

        [Required]
        public int IdContenido { get; set; } // Clave foránea hacia Contenido

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } // Titulo del eventos

        [Required]
        public DateTime FechaEvento { get; set; } // Fecha del evento

        [Required]
        public DateTime HoraInicio { get; set; } // Hora de inicio del evento

        [Required]
        public DateTime HoraFin { get; set; } // Hora de fin del evento

        public string? Imagen { get; set; } // URL o ruta de la imagen del evento

        // Propiedad de navegación hacia Contenido (relación uno a muchos)
        [ForeignKey("IdContenido")]
        public virtual Contenidos Contenido { get; set; }
    }
}
