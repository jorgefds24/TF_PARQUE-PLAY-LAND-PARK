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
    public class JuegosController : Controller
    {
        private readonly MyDbContext _myDbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public JuegosController(MyDbContext myDbContext, IWebHostEnvironment hostingEnvironment)
        {
            _myDbContext = myDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View(_myDbContext.Juegos.ToList());
        }

        public IActionResult Agregar()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Agregar(JuegoViewModel juego)
        {
            // Busca un usuario con IdRol = 1.
            var usuario = _myDbContext.Usuarios.FirstOrDefault(u => u.IdRol == 1);

            if (usuario == null)
            {
                // Si no se encuentra ningún usuario con IdRol = 1, maneja el error.
                ModelState.AddModelError("", "No se encontró ningún usuario con IdRol = 1.");
                return View(juego);
            }

            // Inicia una nueva transacción.
            using (var transaction = _myDbContext.Database.BeginTransaction())
            {
                try
                {              
                    String filename = "";
                    if (juego.Foto != null)
                    {
                        String uploadfolder = Path.Combine(_hostingEnvironment.WebRootPath, "juegos");
                        if (!Directory.Exists(uploadfolder))
                        {
                            Directory.CreateDirectory(uploadfolder);
                        }

                        filename = Guid.NewGuid().ToString() + "_" + juego.Foto.FileName;
                        String filepath = Path.Combine(uploadfolder, filename);

                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            juego.Foto.CopyTo(stream);
                        }

                        Juegos p = new Juegos
                        {
                            Nombre = juego.Nombre,
                            Estado = juego.Estado,
                            Capacidad = juego.Capacidad,
                            Imagen = filename
                        };
                        _myDbContext.Juegos.Add(p);

                        _myDbContext.SaveChanges();

                        //confirma la transacción.
                        transaction.Commit();

                        TempData["Success"] = "Promoción agregada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Foto", "La foto es requerida.");
                        return View(juego);
                    }
                }
                catch (Exception ex)
                {
                    // Si algo ha ido mal, deshace la transacción.
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "Hubo un error al procesar la solicitud.");
                    return View(juego);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> CambiarImagen(int idJuego, IFormFile nuevaImagen)
        {
            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                // Obtén la ruta donde deseas guardar la imagen
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "juegos", nuevaImagen.FileName);

                // Guarda el archivo en la ruta especificada
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await nuevaImagen.CopyToAsync(stream);
                }

                // Actualiza la imagen en la base de datos
                var juego = await _myDbContext.Juegos.FindAsync(idJuego);
                if (juego != null)
                {
                    // Eliminar la imagen anterior si existe
                    if (!string.IsNullOrEmpty(juego.Imagen))
                    {
                        string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "juegos", juego.Imagen);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    juego.Imagen = nuevaImagen.FileName;
                    _myDbContext.Update(juego);
                    await _myDbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var juego = await _myDbContext.Juegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
            }

            var model = new JuegoViewModel
            {

                Nombre = juego.Nombre,
                Estado = juego.Estado,
                Capacidad = juego.Capacidad,
            };

            return View( model);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, JuegoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var juego = await _myDbContext.Juegos.FindAsync(id);
                if (juego == null)
                {
                    return NotFound();
                }

                juego.Nombre = model.Nombre;
                juego.Estado = model.Estado;
                juego.Capacidad = model.Capacidad;

                _myDbContext.Update(juego);
                await _myDbContext.SaveChangesAsync();
                TempData["Success"] = "Juego editado correctamente";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_myDbContext.Juegos.Any(e => e.IdJuego == id))
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
            Juegos juego = await _myDbContext.Juegos.FirstOrDefaultAsync(e => e.IdJuego == Id);
            if (juego == null)
            {
                return NotFound();
            }

            return View(juego);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmarBorrar(int id)
        {
            var juego = await _myDbContext.Juegos.FindAsync(id);
            if (juego == null)
            {
                return NotFound();
            }

            // Eliminar la imagen del juego si existe
            if (!string.IsNullOrEmpty(juego.Imagen))
            {
                string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "juegos", juego.Imagen);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _myDbContext.Juegos.Remove(juego);
            await _myDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}