using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Promociones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPromocion { get; set; } // Clave primaria de Promociones

        [Required]
        public int IdContenido { get; set; } // Clave foránea hacia Contenido

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } // Titulo de la promoción

        [Required]
        public DateTime FechaInicio { get; set; } // Fecha de inicio de la promoción

        [Required]
        public DateTime FechaFin { get; set; } // Fecha de fin de la promoción

        [Column(TypeName = "decimal(10,2)")]
        public decimal Descuento { get; set; } // Descuento de la promoción (ej. 30% o 10%)

        public string? Imagen { get; set; } // URL o ruta de la imagen de la promoción

        // Propiedad de navegación hacia Contenido (relación uno a muchos)
        [ForeignKey("IdContenido")]
        public virtual Contenidos Contenido { get; set; }
    }
}

