using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using photogram.Models;
using photogram.DAL;
using Microsoft.EntityFrameworkCore;

namespace photogram.Controllers
{
    [Authorize] 
    public class UserController : Controller
    {
        private readonly PostDbContext _postDbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(PostDbContext postDbContext, ILogger<UserController> logger)
        {
            _postDbContext = postDbContext;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Feed()
        {
            var posts = await _postDbContext.Posts.ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> Posts(PostViewModel model)
        {
            _logger.LogInformation("Getting all posts");
            var posts = await _postDbContext.Posts.ToListAsync();
            return View(posts);
        }
    }
}
