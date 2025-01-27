namespace photogram.Models;
public class Friend
{
    public int Id { get; set; }
    public string RequesterEmail { get; set; } = string.Empty;
    public string FriendEmail { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
