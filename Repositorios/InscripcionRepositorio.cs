using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class InscripcionRepositorio(EventosContext context)
{
    public async Task<List<Inscripcion>> ListarAsync() => await context.Inscripciones.AsNoTracking()
        .Include(i => i.Evento).Include(i => i.Asistente)
        .OrderByDescending(i => i.FechaInscripcion).ToListAsync();

    public async Task<Evento?> ObtenerEventoAsync(int eventoId) => await context.Eventos.AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id == eventoId && e.Activo);

    public async Task<bool> ConfirmadaExisteAsync(int eventoId, int asistenteId) => await context.Inscripciones.AsNoTracking()
        .AnyAsync(i => i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == "Confirmada");

    public async Task<int> ConfirmadosAsync(int eventoId) => await context.Inscripciones.AsNoTracking()
        .CountAsync(i => i.EventoId == eventoId && i.Estado == "Confirmada");

    public async Task CrearAsync(Inscripcion inscripcion)
    {
        context.Inscripciones.Add(inscripcion);
        await context.SaveChangesAsync();
    }

    public async Task CancelarAsync(int id)
    {
        var inscripcion = await context.Inscripciones.FirstOrDefaultAsync(i => i.Id == id && i.Estado == "Confirmada");
        if (inscripcion is null) return;
        inscripcion.Estado = "Cancelada";
        await context.SaveChangesAsync();
    }
}
