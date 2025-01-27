using photogram.Models;
public class FriendsViewModel
{
    public IEnumerable<User> Users { get; set; } = new List<User>();
    public IEnumerable<Friend> Friends { get; set; } = new List<Friend>();
}
