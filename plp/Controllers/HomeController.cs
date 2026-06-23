using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using plp.Shared.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace plp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5151/api/authentication"); // Asegúrate de que el puerto sea correcto

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                ViewBag.Message = content;
            }
            else
            {
                ViewBag.Message = "Error: " + response.StatusCode;
            }

            return View();
        }
 

        public IActionResult Usuarios()
        {
            return View();
        }

        public IActionResult Juegos()
        {
            return View();
        }

        public IActionResult Tickets()
        {
            return View();
        }

        public IActionResult Ventas()
        {
            return View();
        }

        public IActionResult Eventos()
        {
            return View();
        }

        public IActionResult Promociones()
        {
            return View();
        }

        public IActionResult Comentarios()
        {
            return View();
        }






        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}