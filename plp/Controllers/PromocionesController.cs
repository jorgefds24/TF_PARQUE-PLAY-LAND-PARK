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
    public class PromocionesController : Controller
    {
        private readonly MyDbContext _myDbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PromocionesController(MyDbContext myDbContext, IWebHostEnvironment hostingEnvironment)
        {
            _myDbContext = myDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View(_myDbContext.Promociones.ToList());
        }


        public IActionResult Agregar()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Agregar(PromocionViewModel promocion)
        {
            // Busca un usuario con IdRol = 1.
            var usuario = _myDbContext.Usuarios.FirstOrDefault(u => u.IdRol == 1);

            if (usuario == null)
            {
                // Si no se encuentra ningún usuario con IdRol = 1, maneja el error..
                ModelState.AddModelError("", "No se encontró ningún usuario con IdRol = 1.");
                return View(promocion);
            }

            // Crea un nuevo Contenido.
            Contenidos contenido = new Contenidos
            {
                // IdContenido  es autoincremental.
                IdUsuario = usuario.IdUsuario, // Usa el IdUsuario del usuario con IdRol = 1.
                Tipo = "Promocion" // Indica el tipo de contenido.
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
                        return View(promocion);
                    }

                    String filename = "";
                    if (promocion.Foto != null)
                    {
                        String uploadfolder = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "promociones");
                        if (!Directory.Exists(uploadfolder))
                        {
                            Directory.CreateDirectory(uploadfolder);
                        }

                        filename = Guid.NewGuid().ToString() + "_" + promocion.Foto.FileName;
                        String filepath = Path.Combine(uploadfolder, filename);

                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            promocion.Foto.CopyTo(stream);
                        }

                        Promociones p = new Promociones
                        {
                            IdContenido = idContenido, //IdContenido autoincrementado.
                            Titulo = promocion.Titulo,
                            FechaInicio = promocion.FechaInicio,
                            FechaFin = promocion.FechaFin,
                            Descuento = promocion.Descuento,
                            Imagen = filename
                        };
                        _myDbContext.Promociones.Add(p);

                        _myDbContext.SaveChanges();

                        //confirma la transacción.
                        transaction.Commit();

                        TempData["Success"] = "Promoción agregada correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Foto", "La foto es requerida.");
                        return View(promocion);
                    }
                }
                catch (Exception ex)
                {
                    // Si algo ha ido mal, deshace la transacción.
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "Hubo un error al procesar la solicitud.");
                    return View(promocion);
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> CambiarImagen(int idPromocion, IFormFile nuevaImagen)
        {
            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                // Obtén la ruta donde deseas guardar la imagen
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos" , "promociones", nuevaImagen.FileName);

                // Guarda el archivo en la ruta especificada
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await nuevaImagen.CopyToAsync(stream);
                }

                // Actualiza la imagen en la base de datos
                var promocion = await _myDbContext.Promociones.FindAsync(idPromocion);
                if (promocion != null)
                {
                    // Eliminar la imagen anterior si existe
                    if (!string.IsNullOrEmpty(promocion.Imagen))
                    {
                        string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos" , "promociones", promocion.Imagen);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    promocion.Imagen = nuevaImagen.FileName;
                    _myDbContext.Update(promocion);
                    await _myDbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var promocion = await _myDbContext.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound();
            }

            var model = new PromocionViewModel
            {

                IdContenido = promocion.IdContenido,
                Titulo = promocion.Titulo,
                FechaInicio = promocion.FechaInicio,
                FechaFin = promocion.FechaFin,
                Descuento = promocion.Descuento,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id, PromocionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var promocion = await _myDbContext.Promociones.FindAsync(id);
                if (promocion == null)
                {
                    return NotFound();
                }

                promocion.Titulo = model.Titulo;
                promocion.FechaInicio = model.FechaInicio;
                promocion.FechaFin = model.FechaFin;
                promocion.Descuento = model.Descuento;

                _myDbContext.Update(promocion);
                await _myDbContext.SaveChangesAsync();
                TempData["Success"] = "Promocion editada correctamente";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_myDbContext.Promociones.Any(e => e.IdPromocion == id))
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



        [HttpPost]
        public async Task<IActionResult> ConfirmarBorrar(int id)
        {
            var promocion = await _myDbContext.Promociones.FindAsync(id);
            if (promocion == null)
            {
                return NotFound();
            }

            // Eliminar la imagen del juego si existe
            if (!string.IsNullOrEmpty(promocion.Imagen))
            {
                string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos" ,"promociones", promocion.Imagen);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _myDbContext.Promociones.Remove(promocion);
            await _myDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
