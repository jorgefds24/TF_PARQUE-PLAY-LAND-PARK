using Microsoft.AspNetCore.Mvc;
using plp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace plp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly MyDbContext _myDbContext;

        public UsuariosController (MyDbContext myDbContext)
        {
             _myDbContext = myDbContext;
        }


        [HttpGet]
        public async Task <IActionResult> Lista()
        {
            List<Usuarios> lista = await _myDbContext.Usuarios.Include(u => u.Rol).ToListAsync();
            return View(lista);
        }

        [HttpGet]
        public  IActionResult Nuevo()
        {
            var roles = _myDbContext.Roles.ToList();

            // Pasa los roles a la vista
            ViewBag.Roles = new SelectList(roles, "IdRol", "NombreRol");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(Usuarios usuarios)
        {
            await _myDbContext.Usuarios.AddAsync(usuarios);
            await _myDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int Id)
        {
            var roles = _myDbContext.Roles.ToList();

            // Pasa los roles a la vista
            ViewBag.Roles = new SelectList(roles, "IdRol", "NombreRol");

            Usuarios usuarios = await _myDbContext.Usuarios.FirstAsync(e => e.IdUsuario == Id);
            return View(usuarios);
        }


        [HttpPost]
        public async Task<IActionResult> Editar(Usuarios usuarios)
        {
            // Busca el usuario original en la base de datos
            var originalUser = await _myDbContext.Usuarios.AsNoTracking().FirstAsync(e => e.IdUsuario == usuarios.IdUsuario);

            // Establece la contraseña del usuario actualizado a la contraseña original
            usuarios.Clave = originalUser.Clave;

            // Actualiza el usuario en la base de datos
            _myDbContext.Usuarios.Update(usuarios);
            await _myDbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Lista));
        }


        [HttpGet]
        public async Task<IActionResult> Eliminar(int Id)
        {
            Usuarios usuarios = await _myDbContext.Usuarios.FirstAsync(e => e.IdUsuario == Id);
            _myDbContext.Usuarios.Remove(usuarios);
            await _myDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }



    }
}
