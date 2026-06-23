using System.ComponentModel.DataAnnotations;

namespace plp2.ViewModels
{
    public class UsuarioVM
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } // Nombre del usuario

        [Required(ErrorMessage = "El apellido es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Apellido { get; set; } // Nombre del usuario

        [Required(ErrorMessage = "El correo es requerido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string Correo { get; set; } // Correo del usuario

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder los 100 caracteres")]
        [DataType(DataType.Password)]
        public string Clave { get; set; } // Clave del usuario

        [Required(ErrorMessage = "La confirmación de la contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La confirmación de la contraseña no puede exceder los 100 caracteres")]
        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "La contraseña y la confirmación de la contraseña no coinciden")]
        public string ConfirmarClave { get; set; } // Confirmación de la clave del usuario
    }
}