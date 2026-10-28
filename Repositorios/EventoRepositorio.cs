using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Web.Repositorios;

public class EventoRepositorio(EventosContext context)
{
    public async Task<List<EventoListadoVm>> ListarAsync(int? categoriaId) => await context.Eventos
        .AsNoTracking()
        .Where(e => e.Activo && (!categoriaId.HasValue || e.CategoriaId == categoriaId))
        .OrderBy(e => e.Fecha)
        .Select(e => new EventoListadoVm
        {
            Id = e.Id, Titulo = e.Titulo, Fecha = e.Fecha, Lugar = e.Lugar,
            Aforo = e.Aforo, Precio = e.Precio, Categoria = e.Categoria!.Nombre,
            Modalidad = e.Modalidad,
            Confirmados = e.Inscripciones.Count(i => i.Estado == "Confirmada")
        }).ToListAsync();

    public async Task<Evento?> ObtenerAsync(int id, bool tracking = false)
    {
        var query = context.Eventos.Include(e => e.Categoria).AsQueryable();
        if (!tracking) query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
    }

    public async Task<List<SelectListItem>> CategoriasActivasAsync() => await context.Categorias
        .AsNoTracking().Where(c => c.Activo).OrderBy(c => c.Nombre)
        .Select(c => new SelectListItem(c.Nombre, c.Id.ToString())).ToListAsync();

    public async Task<List<SelectListItem>> EventosActivosAsync() => await context.Eventos
        .AsNoTracking().Where(e => e.Activo).OrderBy(e => e.Fecha)
        .Select(e => new SelectListItem(e.Titulo + " - " + e.Fecha.ToString("dd/MM/yyyy"), e.Id.ToString())).ToListAsync();

    public async Task CrearAsync(Evento evento)
    {
        context.Eventos.Add(evento);
        await context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Evento evento)
    {
        context.Eventos.Update(evento);
        await context.SaveChangesAsync();
    }

    public async Task EliminarLogicoAsync(int id)
    {
        var evento = await context.Eventos.FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        if (evento is null) return;
        evento.Activo = false;
        await context.SaveChangesAsync();
    }
}
