using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace azure_function_example_csharp.ValidationHelpers
{
    public static class ValidationWrapperFactory
    {
        public static ValidationWrapper<T> Create<T>(T model)
        {
            ValidationWrapper<T> body = new ValidationWrapper<T>()
            {
                Value = model
            };            

            var results = new List<ValidationResult>();
            body.IsValid = Validator.TryValidateObject(body.Value, new ValidationContext(body.Value, null, null), results, true);
            body.ValidationResults = results;

            return body;
        }
    }
}
