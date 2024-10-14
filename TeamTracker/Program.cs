using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TeamTracker.Data;
using TeamTracker.Models;
using TeamTracker.Services;
using TeamTracker.Services.Interfaces;



var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add Identity services with the custom user class and roles, and register the data context
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>() // Register the DbContext for Identity
    .AddDefaultTokenProviders();

// Register Razor Pages and MVC services
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Add email configuration
builder.Services.Configure<AuthMessageSenderOptions>(
    builder.Configuration.GetSection("AuthMessageSenderOptions"));

builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddScoped<IImageService, ImageService>();





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseDeveloperExceptionPage();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
