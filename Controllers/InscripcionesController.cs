using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Eventos.Web.Controllers;

public class InscripcionesController(
    InscripcionRepositorio repositorio,
    EventoRepositorio eventoRepositorio,
    AsistenteRepositorio asistenteRepositorio) : Controller
{
    public async Task<IActionResult> Index() => View(await repositorio.ListarAsync());

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new Inscripcion());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        var evento = await repositorio.ObtenerEventoAsync(inscripcion.EventoId);
        if (evento is null || evento.Fecha <= DateTime.Now)
            ModelState.AddModelError(nameof(inscripcion.EventoId), "El evento no existe o ya pasó.");
        if (await repositorio.ConfirmadaExisteAsync(inscripcion.EventoId, inscripcion.AsistenteId))
            ModelState.AddModelError(nameof(inscripcion.AsistenteId), "El asistente ya tiene una inscripción confirmada en este evento.");
        if (evento is not null && await repositorio.ConfirmadosAsync(inscripcion.EventoId) >= evento.Aforo)
            ModelState.AddModelError(nameof(inscripcion.EventoId), "El aforo del evento está completo.");

        if (!ModelState.IsValid)
        {
            await CargarListasAsync();
            return View(inscripcion);
        }

        inscripcion.FechaInscripcion = DateTime.Now;
        inscripcion.Estado = "Confirmada";
        await repositorio.CrearAsync(inscripcion);
        TempData["Mensaje"] = "Inscripción confirmada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        await repositorio.CancelarAsync(id);
        TempData["Mensaje"] = "Inscripción cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        ViewBag.Eventos = await eventoRepositorio.EventosActivosAsync();
        ViewBag.Asistentes = await asistenteRepositorio.AsistentesActivosAsync();
    }
}
