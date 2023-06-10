using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Function.Domain.Models
{
    public class Stock : IValidatableObject
    {
        [Required]
        public string Symbol { get; set; }

        //[Required]
        public string Description {  get; set; }

        //[Range(.01, double.MaxValue, ErrorMessage = "Must be greater than 0")]
        public decimal? Price { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Price < 0)
            {
                yield return new ValidationResult($"Price:{Price} for Symbol:{Symbol} must be greater than 0");
            }

            if (string.IsNullOrEmpty(Description)) 
            {
                yield return new ValidationResult($"Description:{Description} for Symbol:{Symbol} cannot be empty");
            }
        }
    }
}