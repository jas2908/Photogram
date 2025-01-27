using System.Collections.Generic;
using System.Threading.Tasks;
using photogram.Models;

namespace photogram.DAL
{
    public interface IPostRepository
    {
        
        Task<IEnumerable<Post>?> GetAll();
        Task<Post?> GetPostById(int id);
        Task<IEnumerable<Post>?> GetPostsByUserName(string userName);
        Task<int> GetPostsCount();
        Task<IEnumerable<Post>?> GetAllPostsPaged(int? pageNr, int pageSize);
        Task<bool> Create(Post post);
        Task<bool> Update(Post post);
        Task<bool> Delete(int id);

        
        Task<bool> AddFriend(Friend friend);
        Task<bool> RemoveFriend(int id);
        Task<IEnumerable<Friend>> GetFriends(string userEmail);
        Task<IEnumerable<User>> GetAllUsersExcept(string userEmail);
    }
}
