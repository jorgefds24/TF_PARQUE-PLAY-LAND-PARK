using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plp.Shared.Models;


namespace plp2.Controllers
{
    public class JuegosController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public JuegosController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext; // Aquí está la corrección
        }

        // GET: Juegos
        public async Task<IActionResult> Index()
        {
            var juegos= await _myDbContext.Juegos.ToListAsync();
            return View(juegos);
        }
    }
}