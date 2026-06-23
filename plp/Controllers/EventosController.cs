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
    public class EventosController : Controller
    {
        private readonly MyDbContext _myDbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EventosController(MyDbContext myDbContext, IWebHostEnvironment hostingEnvironment)
        {
            _myDbContext = myDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View(_myDbContext.Eventos.ToList());
        }


        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(EventoViewModel evento)
        {
            // Busca un usuario con IdRol = 1.
            var usuario = _myDbContext.Usuarios.FirstOrDefault(u => u.IdRol == 1);

            if (usuario == null)
            {
                // Si no se encuentra ningún usuario con IdRol = 1, maneja el error..
                ModelState.AddModelError("", "No se encontró ningún usuario con IdRol = 1.");
                return View(evento);
            }

            // Crea un nuevo Contenido.
            Contenidos contenido = new Contenidos
            {
                // IdContenido  es autoincremental.
                IdUsuario = usuario.IdUsuario, // Usa el IdUsuario del usuario con IdRol = 1.
                Tipo = "Evento" // Indica el tipo de contenido.
            };

            // Inicia una nueva transacción.
            using (var transaction = _myDbContext.Database.BeginTransaction())
            {
                try
                {
                    // Aca se agrega un nuevo Contenido a la base de datos
                    _myDbContext.Contenidos.Add(contenido);
                    _myDbContext.SaveChanges();

                    // Obtiene el IdContenido autoincrementado.
                    int idContenido = contenido.IdContenido;

                    if (!ModelState.IsValid)
                    {
                        ModelState.AddModelError("", "Datos inválidos.");
                        return View(evento);
                    }

                    String filename = "";
                    if (evento.Foto != null)
                    {
                        String uploadfolder = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "eventos");
                        if (!Directory.Exists(uploadfolder))
                        {
                            Directory.CreateDirectory(uploadfolder);
                        }

                        filename = Guid.NewGuid().ToString() + "_" + evento.Foto.FileName;
                        String filepath = Path.Combine(uploadfolder, filename);

                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            evento.Foto.CopyTo(stream);
                        }

                        Eventos p = new Eventos
                        {
                            IdContenido = idContenido, //IdContenido autoincrementado.
                            Titulo = evento.Titulo,
                            FechaEvento = evento.FechaEvento,
                            HoraInicio = evento.HoraInicio,
                            HoraFin = evento.HoraFin,
                            Imagen = filename
                        };
                        _myDbContext.Eventos.Add(p);

                        _myDbContext.SaveChanges();

                        //confirma la transacción.
                        transaction.Commit();

                        TempData["Success"] = "Evento agregada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Foto", "La foto es requerida.");
                        return View(evento);
                    }
                }
                catch (Exception ex)
                {
                    // Si algo ha ido mal, deshace la transacción.
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "Hubo un error al procesar la solicitud.");
                    return View(evento);
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> CambiarImagen(int idEvento, IFormFile nuevaImagen)
        {
            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + nuevaImagen.FileName;
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "eventos", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await nuevaImagen.CopyToAsync(stream);
                }

                var evento = await _myDbContext.Eventos.FindAsync(idEvento);
                if (evento != null)
                {
                    if (!string.IsNullOrEmpty(evento.Imagen))
                    {
                        string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "eventos", evento.Imagen);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    evento.Imagen = uniqueFileName;
                    _myDbContext.Update(evento);
                    await _myDbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var evento = await _myDbContext.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }

            var model = new EventoViewModel
            {

                IdContenido = evento.IdContenido,
                Titulo = evento.Titulo,
                FechaEvento = evento.FechaEvento,
                HoraInicio = evento.HoraInicio,
                HoraFin = evento.HoraFin,

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, EventoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var evento = await _myDbContext.Eventos.FindAsync(id);
                if (evento == null)
                {
                    return NotFound();
                }

                evento.Titulo = model.Titulo;
                evento.FechaEvento= model.FechaEvento;
                evento.HoraInicio = model.HoraInicio;
                evento.HoraFin = model.HoraFin;

                _myDbContext.Update(evento);
                await _myDbContext.SaveChangesAsync();
                TempData["Success"] = "Evento editada correctamente";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_myDbContext.Eventos.Any(e => e.IdEvento == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Borrar(int Id)
        {
            Eventos evento = await _myDbContext.Eventos.FirstOrDefaultAsync(e => e.IdEvento == Id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }


        [HttpPost, ActionName("ConfirmarBorrar")]
        public async Task<IActionResult> ConfirmarBorrar(int Id)
        {
            Eventos evento = await _myDbContext.Eventos.FindAsync(Id);
            if (evento == null)
            {
                return NotFound();
            }

            // Eliminar la imagen del juego si existe
            if (!string.IsNullOrEmpty(evento.Imagen))
            {
                string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos" , "eventos", evento.Imagen);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _myDbContext.Eventos.Remove(evento);
            await _myDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
