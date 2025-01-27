using System.ComponentModel.DataAnnotations;

namespace photogram.ViewModels
{
    public class UpdatePostRequestModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Content cannot be empty.")]
        [MinLength(1, ErrorMessage = "Content must contain at least one character.")]
        public string? Content { get; set; }
    }
}
