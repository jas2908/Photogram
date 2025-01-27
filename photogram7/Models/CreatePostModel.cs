using System.ComponentModel.DataAnnotations;

namespace photogram.Models
{
    public class CreatePostModel : IValidatableObject
    {
        [StringLength(200, ErrorMessage = "Post content cannot exceed 200 characters.")]
        public string? Content { get; set; } 

        public IFormFile? Image { get; set; } 

        // validation logikk
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Content) && Image == null)
            {
                validationResults.Add(new ValidationResult(
                    "Either content or an image is required.",
                    new[] { nameof(Content), nameof(Image) }
                ));
            }

            return validationResults;
        }
    }
}
