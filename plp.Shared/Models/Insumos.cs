using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Insumos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdInsumo { get; set; } // Clave primaria de Insumos

        [Required]
        public int IdContenido { get; set; } // Clave foránea hacia Contenido

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } // Titulo del insumo

        [Required]
        [StringLength(50)]
        public string TipoInsumo { get; set; } // Tipo de insumo (bebita, comida)

        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; } // Monto del insumo

        public string? Imagen { get; set; } // URL o ruta de la imagen del insumo

        // Propiedad de navegación hacia Contenido (relación uno a muchos)
        [ForeignKey("IdContenido")]
        public virtual Contenidos Contenido { get; set; }
    }
}