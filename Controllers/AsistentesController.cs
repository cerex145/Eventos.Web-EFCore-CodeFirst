using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class AsistentesController(AsistenteRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (await repositorio.DniExisteAsync(asistente.Dni))
            ModelState.AddModelError(nameof(asistente.Dni), "El DNI ya está registrado.");
        if (await repositorio.CorreoExisteAsync(asistente.Correo))
            ModelState.AddModelError(nameof(asistente.Correo), "El correo ya está registrado.");
        if (!ModelState.IsValid) return View(asistente);

        asistente.Activo = true;
        await repositorio.CrearAsync(asistente);
        TempData["Mensaje"] = "Asistente registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
