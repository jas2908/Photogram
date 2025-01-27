using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using photogram.Models;
namespace photogram.DAL;


public class PostDbContext : IdentityDbContext<User>

{
	public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
	{
        //Database.EnsureCreated();
	
	}

	public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Friend> Friends { get; set; }

	 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    if (!optionsBuilder.IsConfigured)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
    }

	
}