using System.ComponentModel.DataAnnotations;

namespace personalManagement.Validaciones
{
    public class PrimeraLetraMayusculaAttribute : ValidationAttribute
    {
        //validar si hay un string
        //si hay string, que la primera letra sea mayuscula
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value == null || string.IsNullOrEmpty(value.ToString())) 
            {
                return ValidationResult.Success;
            } 

            var primeraLetra = value.ToString()[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper()) 
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;

        }
    }
}
