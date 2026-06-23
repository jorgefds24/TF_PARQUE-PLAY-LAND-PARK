using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using plp.Shared.Models;
using plp2.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Configurar el contexto de base de datos para usar la cadena de conexión de appsettings.json
builder.Services.AddDbContext<MyDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Connection"));
    });


//Configurar la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login";
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.LogoutPath = "/Acceso/Logout";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(0);

    });

var app = builder.Build();

// Configurar el proveedor de archivos físicos para acceder a las imágenes en diferentes carpetas de plp
var contentRoot = app.Environment.ContentRootPath;

// Ruta para las imágenes de promociones
var pathToPromociones = Path.Combine(contentRoot, "..", "plp", "wwwroot", "contenidos", "promociones");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(pathToPromociones),
    RequestPath = "/contenidos/promociones"
});

// Ruta para las imágenes de insumos
var pathToInsumos = Path.Combine(contentRoot, "..", "plp", "wwwroot", "contenidos", "insumos");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(pathToInsumos),
    RequestPath = "/contenidos/insumos"
});

// Ruta para las imágenes de eventos
var pathToEventos = Path.Combine(contentRoot, "..", "plp", "wwwroot", "contenidos", "eventos");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(pathToEventos),
    RequestPath = "/contenidos/eventos"
});

// Ruta para las imágenes de los juegos
var pathToJuegos = Path.Combine(contentRoot, "..", "plp", "wwwroot", "juegos");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(pathToJuegos),
    RequestPath = "/juegos"
});


// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//Configurar la autorización
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"));
//});



app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
