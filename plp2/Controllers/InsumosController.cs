using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plp.Shared.Models;

namespace plp2.Controllers
{
    public class InsumosController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public InsumosController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext; // Aquí está la corrección
        }

        // GET: Promociones
        public async Task<IActionResult> Index()
        {
            var insumos = await _myDbContext.Insumos.ToListAsync();
            return View(insumos);
        }
    }
}
