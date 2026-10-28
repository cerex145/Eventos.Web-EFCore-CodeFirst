using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class EventosController(EventoRepositorio repositorio) : Controller
{
    public async Task<IActionResult> Index(int? categoriaId)
    {
        ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
        ViewBag.CategoriaId = categoriaId;
        return View(await repositorio.ListarAsync(categoriaId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        return evento is null ? NotFound() : View(evento);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
        return View(new Evento { Fecha = DateTime.Today.AddDays(7), Modalidad = "Presencial" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
            return View(evento);
        }
        evento.Activo = true;
        await repositorio.CrearAsync(evento);
        TempData["Mensaje"] = "Evento creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        if (evento is null) return NotFound();
        ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
        return View(evento);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await repositorio.CategoriasActivasAsync();
            return View(evento);
        }
        evento.Activo = true;
        await repositorio.ActualizarAsync(evento);
        TempData["Mensaje"] = "Evento actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var evento = await repositorio.ObtenerAsync(id);
        return evento is null ? NotFound() : View(evento);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await repositorio.EliminarLogicoAsync(id);
        TempData["Mensaje"] = "Evento desactivado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
