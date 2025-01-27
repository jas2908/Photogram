namespace photogram.Models;
public class Post
    {
    public int Id { get; set; }
    public string? Content { get; set; }  
    public required string UserName { get; set; } 
    public string? ImagePath { get; set; } 
    public DateTime CreatedAt { get; set; }

    public virtual List<Comment> Comments { get; set; } = new List<Comment>();
    
    }



