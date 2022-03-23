using personalManagement.Validaciones;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace personalManagement.Entidades
{
    public class Autor : IValidatableObject
    {
     
        public int Id { get; set; }

        //{0} es sustituido por la propiedad
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:5, ErrorMessage= "El campo {0} no debe de tener mas de {1} caracteres") ] 
        
        //Validacion personalizada por clases **
        //[PrimeraLetraMayusculaAttribute]
        public string Nombre { get; set; }

        [Range(18,120)]
        [NotMapped] //estan esta propiedad en el modelo pero no en la base de datos
        public int Edad { get; set; }

        [NotMapped]
        public string Tarjeta { get; set; }
        
        [NotMapped]
        public int Menor { get; set; }
        
        [NotMapped]
        public int Mayor { get; set; }

        public List<Libro> Libros { get; set; }


        // se deben pasar todas las reglas por atributos primero 
        // validaciones personalizadas en el modelo
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Nombre)) {
                
                var primerLetra = Nombre[0].ToString();
                if (primerLetra != primerLetra.ToUpper()) {

                    // yield, necesario para poder agregar nuevas validaciones
                    yield return new ValidationResult("La primera letra debe ser en mayuscula", 
                        new string[] {nameof(Nombre) });

                }                
            }

            if (Menor > Mayor) {

                yield return new ValidationResult("este valor no puede ser mas grande que el campo mayor",
                    new string[] { nameof(Menor) }
                    );

            }
        }
    }
}
