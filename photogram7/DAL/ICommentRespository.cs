using System.Collections.Generic;
using System.Threading.Tasks;
using photogram.Models;
using photogram.DAL;


namespace photogram.DAL
{
    public interface ICommentRepository
    {

        Task<bool> AddComment(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsForPost(int postId);
        Task<Comment?> GetCommentById(int id); // Hent en kommentar
        Task<bool> DeleteComment(int id); // Slett en kommentar
        Task<bool> UpdateComment(Comment comment); // Oppdater en kommentar


    }

}