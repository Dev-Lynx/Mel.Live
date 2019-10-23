using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.Extensions.Attributes
{
    public class StringRangeAttribute : ValidationAttribute
    {
        public string[] Range { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Range?.Contains(value?.ToString()) == true)
                return ValidationResult.Success;

            var errorMessage = string.IsNullOrWhiteSpace(ErrorMessage) 
                ? $"Please enter one of the allowable values: {string.Join(", ", (Range ?? new string[] { "No allowable values found" }))}." 
                : ErrorMessage;

            return new ValidationResult(errorMessage);
        }
    }
}
