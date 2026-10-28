using Eventos.Web.Data;
using Eventos.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class AsistenteRepositorio(EventosContext context)
{
    public async Task<List<Asistente>> ListarAsync() => await context.Asistentes.AsNoTracking()
        .Where(a => a.Activo).OrderBy(a => a.Nombres).ToListAsync();

    public async Task<bool> DniExisteAsync(string dni) => await context.Asistentes.AsNoTracking().AnyAsync(a => a.Dni == dni);
    public async Task<bool> CorreoExisteAsync(string correo) => await context.Asistentes.AsNoTracking().AnyAsync(a => a.Correo == correo);

    public async Task<List<SelectListItem>> AsistentesActivosAsync() => await context.Asistentes.AsNoTracking()
        .Where(a => a.Activo).OrderBy(a => a.Nombres)
        .Select(a => new SelectListItem(a.Nombres + " - " + a.Dni, a.Id.ToString())).ToListAsync();

    public async Task CrearAsync(Asistente asistente)
    {
        context.Asistentes.Add(asistente);
        await context.SaveChangesAsync();
    }
}
