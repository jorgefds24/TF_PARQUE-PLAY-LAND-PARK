using Microsoft.AspNetCore.Mvc;
using plp.Shared.Models;
using System.Threading.Tasks;
using plp2.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace plp2.Controllers
{
    public class AccesoController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public AccesoController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            Usuarios usuarios = new Usuarios
            {
                Nombre = modelo.Nombre,
                Apellido = modelo.Apellido,
                Correo = modelo.Correo,
                Clave = modelo.Clave,
                IdRol = 3
            };

            try
            {
                await _myDbContext.Usuarios.AddAsync(usuarios);
                await _myDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ViewData["Mensaje"] = "Ocurrió un error al crear el usuario: " + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewData["Mensaje"] += " Excepción interna: " + ex.InnerException.Message;
                }
                return View();
            }

            if (usuarios.IdUsuario != 0) return RedirectToAction("Login", "Acceso");

            ViewData["Mensaje"] = "No se pudo crear el usuario, error desconocido";
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioVM modelo, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            Usuarios? usuario_encontrado = await _myDbContext.Usuarios.FirstOrDefaultAsync(u => u.Correo == modelo.Correo && u.Clave == modelo.Clave);

            if (usuario_encontrado == null)
            {
                ViewData["mensaje"] = "No se encontró el usuario o la contraseña es incorrecta.";
                return View();
            }

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.Email, usuario_encontrado.Correo),
                new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(0) // Ajusta el tiempo de expiración según tus necesidades
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            if (usuario_encontrado.IdRol == 1 || usuario_encontrado.IdRol == 2) // Asume que 1 es el ID del rol de administrador
            {
                return Redirect("http://localhost:5151");
            }
            else
            {
                returnUrl = returnUrl ?? Url.Action("Index", "Home");
                return LocalRedirect(returnUrl);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso"); // Redirige al usuario a la página de inicio de sesión
        }
    }
}
