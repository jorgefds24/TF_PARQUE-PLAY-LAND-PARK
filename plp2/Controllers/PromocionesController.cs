using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plp.Shared.Models;


namespace plp2.Controllers
{ 
        public class PromocionesController : Controller
        {
            private readonly MyDbContext _myDbContext;

            public PromocionesController(MyDbContext myDbContext)
            {
                _myDbContext = myDbContext; // Aquí está la corrección
            }

        // GET: Promociones
        public async Task<IActionResult> Index()
        {
            var promociones = await _myDbContext.Promociones.ToListAsync();
            return View(promociones);
        }
    }
}   