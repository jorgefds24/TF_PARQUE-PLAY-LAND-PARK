using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Contenidos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdContenido { get; set; } // Clave primaria de Contenido

        [Required]
        public int IdUsuario { get; set; } // Clave foránea hacia Usuarios

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } // Tipo de contenido (promociones, eventos, insumos)


        // Propiedad de navegación hacia Usuarios (relación uno a muchos)
        [ForeignKey("IdUsuario")]
        public virtual Usuarios Usuario { get; set; }

        public virtual ICollection<Promociones> Promociones { get; set; } // Propiedad de navegación para Promociones
        public virtual ICollection<Eventos> Eventos { get; set; } // Propiedad de navegación para Eventos
        public virtual ICollection<Insumos> Insumos { get; set; } // Propiedad de navegación para Insumos
    }
}
