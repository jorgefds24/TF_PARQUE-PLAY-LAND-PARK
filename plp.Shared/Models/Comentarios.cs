using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Comentarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdComentario { get; set; } // Clave primaria autogenerada

        [Required]
        public int IdUsuario { get; set; } // Clave foránea para Usuarios

        [Required]
        public int IdJuego { get; set; } // Clave foránea para Juegos

        [StringLength(255)]
        public string Texto { get; set; } // Texto del comentario

        [Required]
        public DateTime FechaComentario { get; set; } // Fecha del comentario

        [Required]
        [Range(1, 5)]
        public int Calificacion { get; set; } // Calificación del comentario

        [ForeignKey("IdJuego")]
        public virtual Juegos Juego { get; set; } // Propiedad de navegación para la relación con Juegos

        [ForeignKey("IdUsuario")]
        public virtual Usuarios Usuario { get; set; } // Propiedad de navegación para la relación con Usuarios
    }
}