using System.ComponentModel.DataAnnotations;

namespace Template.Backend.Dtos
{
    public class TodoCreationDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        //[PrimeraLetraMayuscula]
        public required string Title { get; set; }
    }
}
