using System;
using System.ComponentModel.DataAnnotations;

namespace photogram.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The comment cannot be empty.")]
        [StringLength(200, ErrorMessage = "The comment cannot exceed 200 characters.")]
        public required string Content { get; set; }

        public required string UserName { get; set; } // Brukernavn
        public DateTime CreatedAt { get; set; } // Opprettet tidspunkt
        public int PostId { get; set; } // ID tilknyttet posten
        public virtual  Post? Post { get; set; } // Navigasjonsegenskap
    }
}
