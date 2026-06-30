using Microsoft.EntityFrameworkCore;
using TheRewind.Models;
using TheRewind.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Connection string: appsettings.json (local dev) or Railway env vars (production)
var cs = builder.Configuration.GetConnectionString("MySqlConnection");

if (string.IsNullOrEmpty(cs))
{
    var host     = Environment.GetEnvironmentVariable("MYSQL_HOST")     ?? "localhost";
    var port     = Environment.GetEnvironmentVariable("MYSQL_PORT")     ?? "3306";
    var database = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "therewinddb";
    var user     = Environment.GetEnvironmentVariable("MYSQL_USER")     ?? "root";
    var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "";
    cs = $"Server={host};Port={port};Database={database};User={user};Password={password};";
}

builder.Services.AddDbContext<ApplicationContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs))
);

builder.Services.AddScoped<IPasswordService, BcryptService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error/500");
    app.UseStatusCodePagesWithReExecute("/error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
