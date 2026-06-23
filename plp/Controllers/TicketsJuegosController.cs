using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using plp.Shared.Models;
using plp.ViewModels; // Asegúrate de tener el espacio de nombres correcto para tu ViewModel
using System.Threading.Tasks;

namespace plp.Controllers
{
    public class TicketsJuegosController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public TicketsJuegosController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        // GET: TicketsJuegos/Index
        public async Task<IActionResult> Index()
        {
            // Agrupa los tickets por nombre y selecciona el primer ticket de cada grupo
            var ticketsUnicosPorNombre = await _myDbContext.Tickets
                .GroupBy(t => t.Nombre) // Agrupa los tickets por nombre
                .Select(g => g.FirstOrDefault()) // Selecciona el primer ticket de cada grupo
                .ToListAsync();

            return View(ticketsUnicosPorNombre);
        }


        // GET: TicketsJuegos/Agregar
        public IActionResult Agregar(int idTicket)
        {
            // Obtén todos los juegos que ya han sido agregados a cualquier ticket
            var juegosAgregados = _myDbContext.TicketsJuegos
                .Select(tj => tj.IdJuego)
                .Distinct()
                .ToList();

            // Filtra los juegos para el SelectList, excluyendo los juegos ya agregados
            var juegosDisponibles = _myDbContext.Juegos
                .Where(j => !juegosAgregados.Contains(j.IdJuego))
                .ToList();

            var viewModel = new TicketJuegoViewModel
            {
                TicketsJuegos = new TicketsJuegos { IdTicket = idTicket },
                Tickets = new SelectList(_myDbContext.Tickets, "IdTicket", "Nombre"),
                Juegos = new SelectList(juegosDisponibles, "IdJuego", "Nombre")
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Agregar(TicketJuegoViewModel viewModel)
        {
            // Verificar si se ha seleccionado un ticket
            if (viewModel.TicketsJuegos.IdTicket == 0)
            {
                // Si no se ha seleccionado un ticket, agregar un mensaje de error al ModelState
                ModelState.AddModelError("TicketsJuegos.IdTicket", "Debes seleccionar un ticket.");
            }
            else if (viewModel.IdsJuegos != null && viewModel.IdsJuegos.Length > 0)
            {
                foreach (var idJuego in viewModel.IdsJuegos)
                {
                    var ticketJuego = new TicketsJuegos
                    {
                        IdTicket = viewModel.TicketsJuegos.IdTicket,
                        IdJuego = idJuego
                    };

                    // Verificar si la combinación de Ticket y Juego ya existe para evitar duplicados
                    var exists = await _myDbContext.TicketsJuegos
                        .AnyAsync(tj => tj.IdTicket == ticketJuego.IdTicket && tj.IdJuego == ticketJuego.IdJuego);

                    if (!exists)
                    {
                        _myDbContext.Add(ticketJuego);
                    }
                }

                await _myDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                // Si idsJuegos es nulo o está vacío, agregar un mensaje de error al ModelState
                ModelState.AddModelError("IdsJuegos", "Debes seleccionar al menos un juego.");
            }

            // Obtener todos los juegos que ya han sido agregados a cualquier ticket
            var juegosAgregados = await _myDbContext.TicketsJuegos
                .Select(tj => tj.IdJuego)
                .Distinct()
                .ToListAsync();

            // Obtener los juegos disponibles excluyendo los que ya están agregados a cualquier ticket
            var juegosDisponibles = await _myDbContext.Juegos
                .Where(j => !juegosAgregados.Contains(j.IdJuego))
                .ToListAsync();

            // Actualizar el SelectList de Juegos con los juegos disponibles filtrados
            viewModel.Juegos = new SelectList(juegosDisponibles, "IdJuego", "Nombre");

            // Actualizar el SelectList de Tickets (opcional, según lo necesites en la vista)
            viewModel.Tickets = new SelectList(_myDbContext.Tickets, "IdTicket", "Nombre");

            return View(viewModel);
        }


        // GET: TicketsJuegos/Detalles/5
        public async Task<IActionResult> Detalles(int idTicket)
        {
            var ticket = await _myDbContext.Tickets
                .Include(t => t.TicketsJuegos)
                .ThenInclude(tj => tj.Juego)
                .FirstOrDefaultAsync(t => t.IdTicket == idTicket);

            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.TicketNombre = ticket.Nombre;
            var juegos = ticket.TicketsJuegos.Select(tj => tj.Juego).ToList();

            return View(juegos);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmarBorrar( int IdJuego)
        {
            var ticketJuego = await _myDbContext.TicketsJuegos
                .FirstOrDefaultAsync(tj => tj.IdJuego == IdJuego);

            if (ticketJuego != null)
            {
                _myDbContext.TicketsJuegos.Remove(ticketJuego);
                await _myDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    }
}
