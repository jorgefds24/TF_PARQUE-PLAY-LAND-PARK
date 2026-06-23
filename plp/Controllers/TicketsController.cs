using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using plp.Shared.Models;
using plp.ViewModels;
using System.Data;
using System.Security.Claims;

namespace plp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly MyDbContext _myDbContext;


        public TicketsController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;

        }

        public IActionResult Index()
        {
            return View(_myDbContext.Tickets.ToList());
        }

        public IActionResult Agregar()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Agregar(TicketViewModel ticket)
        {
            // Busca un usuario con IdRol = 1.
            var usuario = _myDbContext.Usuarios.FirstOrDefault(u => u.IdRol == 1);

            if (usuario == null)
            {
                // Si no se encuentra ningún usuario con IdRol = 1, maneja el error.
                ModelState.AddModelError("", "No se encontró ningún usuario con IdRol = 1.");
                return View(ticket);
            }

            // Inicia una nueva transacción.
            using (var transaction = _myDbContext.Database.BeginTransaction())
            {
                try
                {
                    // Crea una instancia de tu modelo de Ticket basado en el ViewModel
                    Tickets p = new Tickets
                    {
                        Nombre = ticket.Nombre,
                        Precio = ticket.Precio,
                        Color = ticket.Color // Asigna el color seleccionado desde el formulario
                    };

                    // Agrega el nuevo ticket al contexto de la base de datos
                    _myDbContext.Tickets.Add(p);

                    // Guarda los cambios en la base de datos
                    _myDbContext.SaveChanges();

                    // Commit de la transacción
                    transaction.Commit();

                    TempData["Success"] = "Ticket agregado correctamente";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Si algo ha ido mal, deshace la transacción.
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "Hubo un error al procesar la solicitud.");
                    return View(ticket);
                }
            }
        }


        // Acción para mostrar la vista de editar un ticket (GET)
        public IActionResult Editar(int id)
        {
            var ticket = _myDbContext.Tickets.Find(id);

            if (ticket == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el ticket
            }

            var ticketViewModel = new TicketViewModel
            {
                IdTicket = ticket.IdTicket,
                Nombre = ticket.Nombre,
                Precio = ticket.Precio
            };

            return View(ticketViewModel);
        }

        // Acción para procesar la edición de un ticket (POST)
        [HttpPost]
        public IActionResult Editar(TicketViewModel ticket)
        {
            if (ModelState.IsValid)
            {
                // Inicia una nueva transacción
                using (var transaction = _myDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Obtiene el ticket desde la base de datos
                        var ticketToUpdate = _myDbContext.Tickets.Find(ticket.IdTicket);

                        if (ticketToUpdate == null)
                        {
                            return NotFound(); // Retorna 404 si no se encuentra el ticket
                        }

                        // Actualiza las propiedades del ticket con los valores del ViewModel
                        ticketToUpdate.Nombre = ticket.Nombre;
                        ticketToUpdate.Precio = ticket.Precio;
                        ticketToUpdate.Color = ticket.Color; // Actualiza el color

                        // Guarda los cambios en la base de datos
                        _myDbContext.SaveChanges();

                        // Commit de la transacción
                        transaction.Commit();

                        TempData["Success"] = "Ticket actualizado correctamente";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        // Si algo ha ido mal, deshace la transacción
                        transaction.Rollback();
                        Console.WriteLine(ex.ToString());
                        ModelState.AddModelError("", "Hubo un error al procesar la solicitud.");
                        return View(ticket);
                    }
                }
            }
            // Si el modelo no es válido, vuelve a mostrar la vista con los errores
            return View(ticket);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmarBorrar(int id)
        {
            var ticket = await _myDbContext.Tickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }
                    // Remueve el ticket del contexto de la base de datos
                    _myDbContext.Tickets.Remove(ticket);

                    // Guarda los cambios en la base de datos
                    await _myDbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

   
        }
    }
}


