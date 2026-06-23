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
    public class InsumosController : Controller
    {
        private readonly MyDbContext _myDbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InsumosController(MyDbContext myDbContext, IWebHostEnvironment hostingEnvironment)
        {
            _myDbContext = myDbContext;
            _hostingEnvironment = hostingEnvironment;
        }   

        public IActionResult Index()
        {
            return View(_myDbContext.Insumos.ToList());
        }

        
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(InsumoViewModel insumo)
        {
            // Busca un usuario con IdRol = 1.
            var usuario = _myDbContext.Usuarios.FirstOrDefault(u => u.IdRol == 1);

            if (usuario == null)
            {
                // Si no se encuentra ningún usuario con IdRol = 1, maneja el error..
                ModelState.AddModelError("", "No se encontró ningún usuario con IdRol = 1.");
                return View(insumo);
            }

            // Crea un nuevo Contenido.
            Contenidos contenido = new Contenidos
            {
                // IdContenido  es autoincremental.
                IdUsuario = usuario.IdUsuario, // Usa el IdUsuario del usuario con IdRol = 1.
                Tipo = "Insumo" // Indica el tipo de contenido.
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
                        return View(insumo);
                    }

                    String filename = "";
                    if (insumo.Foto != null)
                    {
                        String uploadfolder = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "insumos");
                        if (!Directory.Exists(uploadfolder))
                        {
                            Directory.CreateDirectory(uploadfolder);
                        }

                        filename = Guid.NewGuid().ToString() + "_" + insumo.Foto.FileName;
                        String filepath = Path.Combine(uploadfolder, filename);

                        using (var stream = new FileStream(filepath, FileMode.Create))
                        {
                            insumo.Foto.CopyTo(stream);
                        }

                        Insumos p = new Insumos
                        {
                            IdContenido = idContenido, //IdContenido autoincrementado.
                            Titulo = insumo.Titulo,
                            TipoInsumo = insumo.TipoInsumo,
                            Monto = insumo.Monto,
                            Imagen = filename
                        };
                        _myDbContext.Insumos.Add(p);

                        _myDbContext.SaveChanges();

                        //confirma la transacción.
                        transaction.Commit();

                        TempData["Success"] = "Insumo agregado correctamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Foto", "La foto es requerida.");
                        return View(insumo);
                    }
                }
                catch (Exception ex)
                {
                    // Si algo ha ido mal, deshace la transacción.
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString());
                    ModelState.AddModelError("", "Hubo un error al procesar la solicitud.");
                    return View(insumo);
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> CambiarImagen(int idInsumo, IFormFile nuevaImagen)
        {
            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + nuevaImagen.FileName;
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "insumos", uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await nuevaImagen.CopyToAsync(stream);
                }

                var insumo = await _myDbContext.Insumos.FindAsync(idInsumo);
                if (insumo != null)
                {
                    if (!string.IsNullOrEmpty(insumo.Imagen))
                    {
                        string oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "insumos", insumo.Imagen);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    insumo.Imagen = uniqueFileName;
                    _myDbContext.Update(insumo);
                    await _myDbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var insumo = await _myDbContext.Insumos.FindAsync(id);
            if (insumo == null)
            {
                return NotFound();
            }

            var model = new InsumoViewModel
            {

                IdContenido = insumo.IdContenido,
                Titulo = insumo.Titulo,
                TipoInsumo = insumo.TipoInsumo,
                Monto = insumo.Monto,
            };

            return View(model);
        } 

        [HttpPost]
        public async Task<IActionResult> Editar(int id, InsumoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var insumo = await _myDbContext.Insumos.FindAsync(id);
                if (insumo == null)
                {
                    return NotFound();
                }

                insumo.Titulo = model.Titulo;
                insumo.TipoInsumo = model.TipoInsumo;
                insumo.Monto = model.Monto;

                _myDbContext.Update(insumo);
                await _myDbContext.SaveChangesAsync();
                TempData["Success"] = "Insumo editado correctamente";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_myDbContext.Insumos.Any(e => e.IdInsumo == id))
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
            Insumos insumo = await _myDbContext.Insumos.FirstOrDefaultAsync(e => e.IdInsumo == Id);
            if (insumo == null)
            {
                return NotFound();
            }

            return View(insumo);
        }


        [HttpPost, ActionName("ConfirmarBorrar")]
        public async Task<IActionResult> ConfirmarBorrar(int Id)
        {
            Insumos insumo = await _myDbContext.Insumos.FindAsync(Id);
            if (insumo == null)
            {
                return NotFound();
            }

            // Eliminar la imagen del juego si existe
            if (!string.IsNullOrEmpty(insumo.Imagen))
            {
                string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "contenidos", "insumos", insumo.Imagen);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _myDbContext.Insumos.Remove(insumo);
            await _myDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}

