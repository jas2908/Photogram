using Microsoft.AspNetCore.Http;
namespace photogram.Models
{


    public class PostViewModel
    {
        public int Id { get; set; }
        public string? Content { get; set; } // Kun caption skal redigeres

        public string? ImagePath { get; set; } // Kun for visning av eksisterende bilde*/
    }

    public class UpdatePostRequestModel
    {
        public int Id { get; set; } // For å identifisere posten
        public string? Content { get; set; } // Kun caption skal redigeres

    }
}




