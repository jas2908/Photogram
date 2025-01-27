using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using photogram.Controllers;
using photogram.DAL;
using photogram.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace photogram.Tests.Controllers
{
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<ILogger<PostController>> _mockLogger;
        private readonly PostController _controller;

        public PostControllerTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockLogger = new Mock<ILogger<PostController>>();

            _controller = new PostController(
                _mockPostRepository.Object,
                _mockLogger.Object
            );
        }

        // Helper method to mock the user identity
        private void SetUser(string userName)
        {
            var user = new Mock<System.Security.Principal.IIdentity>();
            user.Setup(u => u.Name).Returns(userName);

            var context = new Mock<HttpContext>();
            context.Setup(ctx => ctx.User.Identity).Returns(user.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context.Object
            };
        }

        // TEST: CreatePost
        [Fact]
        public async Task CreatePost_ShouldRedirectToMyPosts_WhenModelIsValid()
        {
            // Arrange
            SetUser("test@example.com");

            var model = new CreatePostModel { Content = "Test Post", Image = null };
            _mockPostRepository.Setup(repo => repo.Create(It.Is<Post>(p =>
                p.Content == "Test Post" &&
                p.UserName == "test@example.com"
            ))).ReturnsAsync(true);

            // Act
            var result = await _controller.CreatePost(model);

            // Assert
            _mockPostRepository.Verify(repo => repo.Create(It.IsAny<Post>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyPosts", redirectResult.ActionName);
        }

        [Fact]
        public async Task CreatePost_ShouldReturnView_WhenRepositoryFailsToCreatePost()
        {
            // Arrange
            SetUser("test@example.com");

            var model = new CreatePostModel { Content = "Test Post", Image = null };
            _mockPostRepository.Setup(repo => repo.Create(It.Is<Post>(p =>
                p.Content == "Test Post" &&
                p.UserName == "test@example.com"
            ))).ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePost(model);

            // Assert
            _mockPostRepository.Verify(repo => repo.Create(It.IsAny<Post>()), Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(model, viewResult.Model);
        }

        // TEST: Feed
        [Fact]
        public async Task Feed_ShouldReturnViewWithPosts()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post { Id = 1, Content = "Post 1", UserName = "test@example.com", CreatedAt = DateTime.Now }
            };
            _mockPostRepository.Setup(repo => repo.GetAllPostsPaged(It.IsAny<int?>(), 10)).ReturnsAsync(posts);

            // Act
            var result = await _controller.Feed(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(posts, viewResult.Model);
        }

        [Fact]
        public async Task Feed_ShouldReturnStatusCode500_WhenExceptionOccurs()
        {
            // Arrange
            _mockPostRepository.Setup(repo => repo.GetAllPostsPaged(It.IsAny<int?>(), 10))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Feed(1);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // TEST: UpdatePost
        [Fact]
        public async Task UpdatePost_ShouldRedirectToMyPosts_WhenUpdateIsSuccessful()
        {
            // Arrange
            SetUser("user@example.com");

            var model = new UpdatePostRequestModel { Id = 1, Content = "Updated Content" };
            var post = new Post
            {
                Id = 1,
                Content = "Old Content",
                UserName = "user@example.com",
                CreatedAt = DateTime.Now
            };
            _mockPostRepository.Setup(repo => repo.GetPostById(1)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.Update(It.IsAny<Post>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePost(model);

            // Assert
            _mockPostRepository.Verify(repo => repo.Update(It.IsAny<Post>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyPosts", redirectResult.ActionName);
        }

        [Fact]
public async Task UpdatePost_ShouldReturnNotFound_WhenPostDoesNotExist()
{
    // Arrange
    SetUser("user@example.com");

    var model = new UpdatePostRequestModel { Id = 1, Content = "Updated Content" };
    _mockPostRepository.Setup(repo => repo.GetPostById(1)).ReturnsAsync((Post?)null);

    // Act
    var result = await _controller.UpdatePost(model);

    // Assert
    Assert.IsType<NotFoundObjectResult>(result);
}


        // TEST: DeletePost
        [Fact]
        public async Task DeletePost_ShouldRedirectToMyPosts_WhenDeleteIsSuccessful()
        {
            // Arrange
            SetUser("user@example.com");

            var post = new Post
            {
                Id = 1,
                UserName = "user@example.com",
                CreatedAt = DateTime.Now
            };
            _mockPostRepository.Setup(repo => repo.GetPostById(1)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.Delete(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePost(1);

            // Assert
            _mockPostRepository.Verify(repo => repo.Delete(1), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("MyPosts", redirectResult.ActionName);
        }
[Fact]
public async Task DeletePost_ShouldReturnNotFound_WhenPostDoesNotExist()
{
    // Arrange
    SetUser("user@example.com");

    _mockPostRepository.Setup(repo => repo.GetPostById(1)).ReturnsAsync((Post?)null);

    // Act
    var result = await _controller.DeletePost(1);

    // Assert
    Assert.IsType<NotFoundObjectResult>(result);
}

    }
}
