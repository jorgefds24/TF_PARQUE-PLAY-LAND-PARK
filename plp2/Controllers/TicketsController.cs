using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using plp.Shared.Models;


namespace plp2.Controllers
{
    public class TicketsController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public TicketsController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext; // Aquí está la corrección
        }

        // GET: Promociones
        public async Task<IActionResult> Index()
        {
            var tickets = await _myDbContext.Tickets.ToListAsync();
            return View(tickets);
        }
    }
}