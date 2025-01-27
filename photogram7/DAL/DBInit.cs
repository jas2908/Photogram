using Microsoft.EntityFrameworkCore;
using photogram.Models;

namespace photogram.DAL;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<PostDbContext>();

        // Reset database 
        //context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Seed Posts
        if (!context.Posts.Any())
        {
            var posts = new List<Post>
            {
                new Post
                {
                    Content = "Exploring the beautiful landscapes of Norway!",
                    UserName = "wanderlust_jane",
                    ImagePath = "/images/norway.jpg",
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Post
                {
                    Content = "Had an amazing time at the tech conference!",
                    UserName = "dev_john",
                    ImagePath = "/images/conference.jpg",
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Post
                {
                    Content = "Sunsets like these make everything worth it.",
                    UserName = "sunset_lover",
                    ImagePath = "/images/sunset.jpg",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new Post
                {
                    Content = "Trying out new recipes today. Cooking vibes!",
                    UserName = "chef_sara",
                    ImagePath = "/images/recipes.jpg",
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            };
            context.AddRange(posts);
            context.SaveChanges();
        }

        
        if (!context.Comments.Any())
        {
            var comments = new List<Comment>
            {
                new Comment
                {
                    Content = "This looks amazing!",
                    PostId = 1, 
                    UserName = "travel_enthusiast",
                    CreatedAt = DateTime.Now.AddDays(-8)
                },
                new Comment
                {
                    Content = "I want to visit Norway someday!",
                    PostId = 1,
                    UserName = "dreamer123",
                    CreatedAt = DateTime.Now.AddDays(-7)
                }
            };
            context.AddRange(comments);
            context.SaveChanges();
        }
    }
}
