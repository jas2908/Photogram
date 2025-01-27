using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using photogram.Models;
using photogram.DAL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;

namespace photogram.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostController> _logger;

        private readonly string _uploadsFolder = "wwwroot/uploads"; 

        public PostController(IPostRepository postRepository, ILogger<PostController> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost(CreatePostModel model)
        {
            _logger.LogInformation("Creating a new post in PostController");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while creating a post.");
                return View(model);
            }

            try
            {
                var userEmail = User.Identity?.Name;

                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogWarning("User is not logged in while creating a post.");
                    return RedirectToAction("Login", "User");
                }

                var post = new Post
                {
                    Content = string.IsNullOrWhiteSpace(model.Content) ? null : model.Content,
                    CreatedAt = DateTime.Now,
                    UserName = userEmail
                };

                if (model.Image != null)
                {
                    Directory.CreateDirectory(_uploadsFolder); 
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                    var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }

                    post.ImagePath = "/uploads/" + uniqueFileName;
                }

                if (string.IsNullOrWhiteSpace(post.Content) && string.IsNullOrWhiteSpace(post.ImagePath))
                {
                    ModelState.AddModelError("", "Either content or an image is required to create a post.");
                    return View(model);
                }

                bool created = await _postRepository.Create(post);

                if (!created)
                {
                    _logger.LogError("Failed to create the post.");
                    ModelState.AddModelError("", "Failed to create the post."); // Legg til en feilmelding i ModelState
                    return View(model); // Returner skjemaet med brukers data
                }   

                return RedirectToAction("MyPosts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the post.");
                return StatusCode(500, "An error occurred while creating the post.");
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> Feed(int? pageNr, int? editingCommentId = null)
        {
            try
            {
                var posts = await _postRepository.GetAllPostsPaged(pageNr, 10);
                if (editingCommentId.HasValue)
                {
                    ViewData["EditingCommentId"] = editingCommentId.Value;
                }
                return View(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching the feed.");
                return StatusCode(500, "An error occurred while fetching the feed.");
            }
        }

        public async Task<IActionResult> MyPosts(int? editingPostId = null)
{
    try
    {
        var userEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return RedirectToAction("Login", "User");
        }

        var userPosts = await _postRepository.GetPostsByUserName(userEmail);
        ViewBag.EditingPostId = editingPostId;
        return View(userPosts); // Returnerer en liste av innlegg
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while fetching user posts.");
        return StatusCode(500, "An error occurred while fetching user posts.");
    }
}


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditPost(int id)
        {
            _logger.LogInformation("EditPost GET called with ID: {Id}", id);

            try
            {
                var post = await _postRepository.GetPostById(id);
                if (post == null)
                {
                    _logger.LogWarning("Post with ID {Id} not found", id);
                    return NotFound("The requested post was not found.");
                }

                return RedirectToAction("MyPosts", new { editingPostId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching the post for editing.");
                return StatusCode(500, "An error occurred while fetching the post for editing.");
            }
        }

        
[HttpPost]
[Authorize]
public async Task<IActionResult> UpdatePost(UpdatePostRequestModel model)
{
    _logger.LogInformation("UpdatePost called for post ID: {Id}", model.Id);

    // Ekstra validering for å sikre at Content ikke er tomt
    if (string.IsNullOrWhiteSpace(model.Content))
    {
        ModelState.AddModelError("Content", "Content cannot be empty.");
        return RedirectToAction("MyPosts", new { editingPostId = model.Id });
    }

    if (!ModelState.IsValid)
    {
        _logger.LogWarning("Invalid model state for post ID: {Id}", model.Id);
        return RedirectToAction("MyPosts", new { editingPostId = model.Id });
    }

    try
    {
        var post = await _postRepository.GetPostById(model.Id);
        if (post == null)
        {
            _logger.LogWarning("Post with ID {Id} not found during update", model.Id);
            return NotFound("The post was not found.");
        }

        var loggedInUserEmail = User.Identity?.Name;
        if (post.UserName != loggedInUserEmail)
        {
            _logger.LogWarning("Unauthorized update attempt by user {UserEmail} for post ID: {Id}", loggedInUserEmail, model.Id);
            return Forbid("You are not authorized to update this post.");
        }

        post.Content = model.Content;

        var updateSuccess = await _postRepository.Update(post);
        if (!updateSuccess)
        {
            _logger.LogError("Failed to update post with ID: {Id}", model.Id);
            ModelState.AddModelError("", "An error occurred while updating the post. Please try again.");
            return RedirectToAction("MyPosts", new { editingPostId = model.Id });
        }

        _logger.LogInformation("Post with ID {Id} successfully updated by user {UserEmail}", model.Id, loggedInUserEmail);
        return RedirectToAction("MyPosts");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while updating the post with ID {Id}", model.Id);
        ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
        return RedirectToAction("MyPosts", new { editingPostId = model.Id });
    }
}

         




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var post = await _postRepository.GetPostById(id);
                if (post == null)
                {
                    return NotFound("The post was not found for deletion.");
                }

                var loggedInUserEmail = User.Identity?.Name;
                if (post.UserName != loggedInUserEmail)
                {
                    _logger.LogWarning("Unauthorized delete attempt by user {UserEmail} for post ID: {Id}", loggedInUserEmail, id);
                    return Forbid("You are not authorized to delete this post.");
                }

                var isDeleted = await _postRepository.Delete(id);
                if (!isDeleted)
                {
                    return StatusCode(500, "Failed to delete the post.");
                }

                return RedirectToAction("MyPosts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the post.");
                return StatusCode(500, "An error occurred while deleting the post.");
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Friends()
        {
            var userEmail = User.Identity?.Name;

            if (userEmail == null)
            {
                _logger.LogWarning("User is not logged in.");
                return RedirectToAction("Login", "User");
            }

            var allUsers = await _postRepository.GetAllUsersExcept(userEmail);
            var friends = await _postRepository.GetFriends(userEmail);

            var nonFriends = allUsers.Where(u => !friends.Any(f => f.FriendEmail == u.Email)).ToList();

            var viewModel = new FriendsViewModel
            {
                Users = nonFriends,
                Friends = friends
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFriend(string friendEmail)
        {
            var userEmail = User.Identity?.Name;

            if (userEmail == null || string.IsNullOrEmpty(friendEmail))
            {
                _logger.LogWarning("User is not logged in or friendEmail is empty.");
                return RedirectToAction("Login", "User");
            }

            var existingFriendship = (await _postRepository.GetFriends(userEmail))
                .Any(f => f.FriendEmail == friendEmail);

            if (!existingFriendship)
            {
                var friend = new Friend
                {
                    RequesterEmail = userEmail,
                    FriendEmail = friendEmail
                };

                await _postRepository.AddFriend(friend);
            }

            return RedirectToAction("Friends");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFriend(int id)
        {
            var userEmail = User.Identity?.Name;

            if (userEmail == null)
            {
                _logger.LogWarning("User is not logged in.");
                return RedirectToAction("Login", "User");
            }

            await _postRepository.RemoveFriend(id);

            return RedirectToAction("Friends");
        }
    }
}

