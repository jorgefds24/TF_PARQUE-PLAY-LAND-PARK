using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace plp.Shared.Models
{
    public class Usuarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; } // Clave primaria autogenerada

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } // Nombre del usuario

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } // Apellido del usuario

        [Required]
        [StringLength(100)]
        public string Correo { get; set; } // Correo del usuario

        [Required]
        [StringLength(100)]
        public string Clave { get; set; } // Clave del usuario

        [Required]
        public int IdRol { get; set; }

        [ForeignKey("IdRol")]
        public virtual Roles Rol { get; set; } // Propiedad de navegación para Rol

        public virtual ICollection<Contenidos> Contenidos { get; set; }

        public virtual ICollection<ComprasClientes> ComprasClientes { get; set; }

        public virtual ICollection<Comentarios> Comentarios{ get; set; }
    }
}