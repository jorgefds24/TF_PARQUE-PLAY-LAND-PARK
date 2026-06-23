using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using plp.Shared.Models;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// Configuración de la cadena de conexión
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login";
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.LogoutPath = "/Sesion/Logout";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(0);
    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
//    // Agrega más políticas según necesites
//});

// Agregar servicios al contenedor
builder.Services.AddHttpClient(); // Agrega el cliente HTTP si es necesario
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
