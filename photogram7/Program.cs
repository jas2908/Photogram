using Microsoft.EntityFrameworkCore;
using photogram.DAL;
using photogram.Models;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostDbContextConnection") ?? throw new InvalidOperationException("Connection string 'PostDbContextConnection' not found.");;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the SQLite database contexts
builder.Services.AddDbContext<PostDbContext>(options =>
{
    options.UseLazyLoadingProxies() // Enable lazy loading if needed
           .UseSqlite(builder.Configuration["ConnectionStrings:PostDbContextConnection"]);
});

builder.Services.AddDefaultIdentity<User>().AddEntityFrameworkStores<PostDbContext>();

builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddRazorPages(); //order of adding service does not matter
builder.Services.AddSession();


// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the timeout for the session
    options.Cookie.HttpOnly = true; // Set the cookie to be HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

builder.Services.AddHttpContextAccessor();

// Configure Serilog for logging
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
                            e.Level == LogEventLevel.Information &&
                            e.MessageTemplate.Text.Contains("Executed DbCommand"));
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Seed the database in development environment
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();

// Add session middleware before routing 
app.UseSession(); // Ensure this is called before app.UseRouting()

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


// Define the default route for the application
app.MapDefaultControllerRoute();

app.MapRazorPages();

app.Run();
