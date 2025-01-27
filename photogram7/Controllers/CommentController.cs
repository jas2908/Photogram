using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using photogram.Models;
using photogram.DAL;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace photogram.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentRepository commentRepository, ILogger<CommentsController> logger)
        {
            _commentRepository = commentRepository;
            _logger = logger;
        }

       [HttpPost]
[Authorize]
public async Task<IActionResult> AddComment(int postId, string content)
{
    if (string.IsNullOrWhiteSpace(content))
    {
        TempData["CommentValidationError"] = "Comment cannot be empty.";
        return RedirectToAction("Feed", "Post"); // Eller hvor kommentarene vises
    }

    try
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "User");
        }

        var comment = new Comment
        {
            PostId = postId,
            Content = content,
            UserName = userEmail,
            CreatedAt = DateTime.Now
        };

        bool created = await _commentRepository.AddComment(comment);

        if (!created)
        {
            TempData["CommentValidationError"] = "Failed to add comment.";
            return RedirectToAction("Feed", "Post");
        }

        return RedirectToAction("Feed", "Post");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error while adding comment.");
        TempData["CommentValidationError"] = "An unexpected error occurred.";
        return RedirectToAction("Feed", "Post");
    }
}


        [HttpGet]
        public async Task<IActionResult> GetComments(int postId)
        {
            try
            {
                var comments = await _commentRepository.GetCommentsForPost(postId);
                return PartialView("_CommentsPartial", comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching comments for PostId {PostId}.", postId);
                return StatusCode(500, "Could not fetch comments.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                var comment = await _commentRepository.GetCommentById(id);
                if (comment == null)
                {
                    _logger.LogWarning("Comment with Id {Id} not found for deletion.", id);
                    return NotFound("The comment was not found.");
                }

                // Ensure the logged-in user is the owner of the comment
                var loggedInUserEmail = User.Identity?.Name;
                if (comment.UserName != loggedInUserEmail)
                {
                    _logger.LogWarning("Unauthorized delete attempt by User {UserEmail} for CommentId {CommentId}.", loggedInUserEmail, id);
                    return Forbid("You are not authorized to delete this comment.");
                }

                var success = await _commentRepository.DeleteComment(id);

                if (!success)
                {
                    _logger.LogError("Failed to delete CommentId {CommentId}.", id);
                    return StatusCode(500, "Failed to delete the comment.");
                }

                _logger.LogInformation("CommentId {CommentId} deleted successfully.", id);
                return RedirectToAction("Feed", "Post");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting CommentId {CommentId}.", id);
                return StatusCode(500, "An error occurred while deleting the comment.");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditComment(int id, string updatedContent)
        {
            if (string.IsNullOrWhiteSpace(updatedContent))
            {
                _logger.LogWarning("Attempted to save an empty comment for CommentId {CommentId}.", id);
                TempData["EditCommentValidationError"] = "The comment cannot be empty.";
                return RedirectToAction("Feed", "Post", new { editingCommentId = id });
            }

            try
            {
                var comment = await _commentRepository.GetCommentById(id);

                if (comment == null)
                {
                    _logger.LogWarning("Comment with Id {CommentId} not found for editing.", id);
                    return NotFound("The comment was not found.");
                }

                var loggedInUserEmail = User.Identity?.Name;

                if (comment.UserName != loggedInUserEmail)
                {
                    _logger.LogWarning("Unauthorized edit attempt by User {UserEmail} for CommentId {CommentId}.", loggedInUserEmail, id);
                    return Forbid("You are not authorized to edit this comment.");
                }

                comment.Content = updatedContent;

                var success = await _commentRepository.UpdateComment(comment);

                if (!success)
                {
                    _logger.LogError("Failed to update CommentId {CommentId} by User {UserEmail}.", id, loggedInUserEmail);
                    TempData["EditCommentError"] = "Failed to update the comment.";
                    return RedirectToAction("Feed", "Post", new { editingCommentId = id });
                }

                _logger.LogInformation("CommentId {CommentId} successfully updated by User {UserEmail}.", id, loggedInUserEmail);
                return RedirectToAction("Feed", "Post");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating CommentId {CommentId} by User {UserEmail}.", id, User.Identity?.Name);
                TempData["EditCommentError"] = "An error occurred while updating the comment.";
                return RedirectToAction("Feed", "Post", new { editingCommentId = id });
            }
        }
    }
}
