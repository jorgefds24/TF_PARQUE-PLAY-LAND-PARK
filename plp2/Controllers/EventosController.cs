using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plp.Shared.Models;


namespace plp2.Controllers
{
    public class EventosController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public EventosController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext; // Aquí está la corrección
        }

        // GET: Promociones
        public async Task<IActionResult> Index()
        {
            var eventos = await _myDbContext.Eventos.ToListAsync();
            return View(eventos);
        }
    }
}