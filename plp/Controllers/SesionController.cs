using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using plp.Shared.Models;



namespace plp.Controllers
{
    [Authorize]
    public class SesionController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public SesionController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }


        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        //[HttpPost]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //    return Redirect("http://localhost:5250/Acceso/Login"); // Redirige al usuario a la página de inicio de sesión de plp2
        //}

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoggedOut", "Sesion"); // Redirige al usuario a la página de "Sesión cerrada"
        }


        public IActionResult LoggedOut()
        {
            return View();
        }

    }
}
