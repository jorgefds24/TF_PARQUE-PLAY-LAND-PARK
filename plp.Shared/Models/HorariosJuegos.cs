using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace plp.Shared.Models
{
    public class HorariosJuegos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHorarioJuego { get; set; }

        [Required]
        public DateTime HoraInicio { get; set; }

        [Required]
        public DateTime HoraFin { get; set; }

        // Clave foránea para Juegos
        public int IdJuego { get; set; } 

        // Propiedad de navegación hacia Juegos
        [ForeignKey("IdJuego")]
        public virtual Juegos Juego { get; set; }
    }
}
