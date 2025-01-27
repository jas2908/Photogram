using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using photogram.Models;
using Microsoft.Extensions.Logging;

namespace photogram.DAL
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PostDbContext _db;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(PostDbContext db, ILogger<CommentRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> AddComment(Comment comment)
        {
            try
            {
                await _db.Comments.AddAsync(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] AddComment failed for Comment {@Comment}: {ErrorMessage}", comment, e.Message);
                return false;
            }
        }

        public async Task<IEnumerable<Comment>> GetCommentsForPost(int postId)
        {
            try
            {
                return await _db.Comments
                    .Where(c => c.PostId == postId)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] GetCommentsForPost failed for PostId {PostId}: {ErrorMessage}", postId, e.Message);
                return Enumerable.Empty<Comment>();
            }
        }

        public async Task<Comment?> GetCommentById(int id)
        {
            try
            {
                return await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] GetCommentById failed for CommentId {CommentId}: {ErrorMessage}", id, e.Message);
                return null;
            }
        }

        public async Task<bool> DeleteComment(int id)
        {
            try
            {
                var comment = await _db.Comments.FindAsync(id);
                if (comment == null)
                {
                    _logger.LogWarning("[CommentRepository] DeleteComment failed: CommentId {CommentId} not found", id);
                    return false;
                }

                _db.Comments.Remove(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] DeleteComment failed for CommentId {CommentId}: {ErrorMessage}", id, e.Message);
                return false;
            }
        }

        public async Task<bool> UpdateComment(Comment comment)
        {
            try
            {
                _db.Comments.Update(comment);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("[CommentRepository] UpdateComment failed for Comment {@Comment}: {ErrorMessage}", comment, e.Message);
                return false;
            }
        }
    }
}
