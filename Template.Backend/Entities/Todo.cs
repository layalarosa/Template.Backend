using System.ComponentModel.DataAnnotations;
using Template.Backend.Validations;

namespace Template.Backend.Entities
{
    public class Todo
    {
        public int Id { get; set; }
        // Placeholder {0}{1} utilizarlos en las validaciones
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
        [FirstCapitalLetter] 
        public required string Title { get; set; }

        // Ejemplos de validaciones Backend
        
        //[Range(18, 120)]
        //public int Edad {  get; set; }

        // Esto no valida que sea una tarjeta de credito real
        //[CreditCard]
        //public string? CreditCard { get; set; }
    }
}
