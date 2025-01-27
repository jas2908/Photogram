using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using photogram.DAL;
using photogram.Models;
using System.Threading.Tasks;

namespace photogram.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IPostRepository postRepository, ILogger<HomeController> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Feed()
        {
            return RedirectToAction("Feed", "Post");
        }

         public IActionResult Friends()
        {
            return RedirectToAction("Friends", "Post"); 
        }

        public IActionResult MyPosts()
        {
            return RedirectToAction("MyPosts", "Post");
        }

         
    }
}



