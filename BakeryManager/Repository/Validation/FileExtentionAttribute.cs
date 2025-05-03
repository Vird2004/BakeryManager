using System.ComponentModel.DataAnnotations;

namespace BakeryManager.Repository.Validation
{
    public class FileExtentionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file) 
            {
                var extension = Path.GetExtension(file.FileName); //123.jpg
                string[] extensions = { "jpg", "png", "jpeg"};

                bool result = extensions.Any(x=>extension.EndsWith(x));

                if (!result)
                {
                    return new ValidationResult($"File type not supported. Supported types are: {string.Join(", ", extensions)}");
                }
            }
            return ValidationResult.Success;
        }
    }
    
}

