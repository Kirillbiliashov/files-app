using System.Text.Json.Serialization;
using FilesApp.DAL;
using FilesApp.Models.DAL;
using FilesApp.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews()
.AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddAuthorization();
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<FilesAppDbContext>();

builder.Services.AddDbContext<FilesAppDbContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))
    .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

builder.Services.AddScoped<IFilesRepository, FilesRepository>();
builder.Services.AddScoped<IFoldersRepository, FoldersRepository>();
builder.Services.AddScoped<IItemsRepository, ItemsRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToFile("index.html");

app.Run();
