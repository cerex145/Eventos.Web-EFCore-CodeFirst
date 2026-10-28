using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EventosContext>(options =>
{
    if (builder.Configuration.GetValue<bool>("UseInMemory"))
        options.UseInMemoryDatabase("EventosDemo");
    else
        options.UseSqlServer(builder.Configuration.GetConnectionString("EventosConnection"));
});
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();
    if (context.Database.IsRelational())
        await context.Database.MigrateAsync();

    if (!await context.Categorias.AnyAsync())
    {
        var categorias = new[]
        {
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Negocios" },
            new Categoria { Nombre = "Educación" }
        };
        await context.Categorias.AddRangeAsync(categorias);
        await context.SaveChangesAsync();

        await context.Eventos.AddRangeAsync(
            new Evento { Titulo = "Conferencia de Inteligencia Artificial", Fecha = DateTime.Today.AddDays(14).AddHours(9), Lugar = "Auditorio Central", Aforo = 120, Precio = 45, CategoriaId = categorias[0].Id, Modalidad = "Presencial" },
            new Evento { Titulo = "Taller de ASP.NET Core", Fecha = DateTime.Today.AddDays(21).AddHours(15), Lugar = "Laboratorio 3", Aforo = 35, Precio = 30, CategoriaId = categorias[0].Id, Modalidad = "Presencial" },
            new Evento { Titulo = "Foro de Emprendimiento Digital", Fecha = DateTime.Today.AddDays(30).AddHours(18), Lugar = "Microsoft Teams", Aforo = 300, Precio = 0, CategoriaId = categorias[1].Id, Modalidad = "Virtual" },
            new Evento { Titulo = "Seminario de Innovación Educativa", Fecha = DateTime.Today.AddDays(45).AddHours(10), Lugar = "Zoom", Aforo = 200, Precio = 15, CategoriaId = categorias[2].Id, Modalidad = "Virtual" });
        await context.SaveChangesAsync();
    }

    if (!await context.Asistentes.AnyAsync())
    {
        await context.Asistentes.AddRangeAsync(
            new Asistente { Nombres = "Ana Torres", Dni = "70123456", Correo = "ana.torres@example.com", Telefono = "987654321" },
            new Asistente { Nombres = "Bruno Salazar", Dni = "71234567", Correo = "bruno.salazar@example.com", Telefono = "986543210" },
            new Asistente { Nombres = "Carla Mendoza", Dni = "72345678", Correo = "carla.mendoza@example.com", Telefono = "985432109" },
            new Asistente { Nombres = "Diego Ramos", Dni = "73456789", Correo = "diego.ramos@example.com", Telefono = "984321098" },
            new Asistente { Nombres = "Elena Vargas", Dni = "74567890", Correo = "elena.vargas@example.com", Telefono = "983210987" });
        await context.SaveChangesAsync();
    }

    if (!await context.Inscripciones.AnyAsync())
    {
        var eventos = await context.Eventos.OrderBy(e => e.Id).Take(3).ToListAsync();
        var asistentes = await context.Asistentes.OrderBy(a => a.Id).Take(3).ToListAsync();
        if (eventos.Count == 3 && asistentes.Count == 3)
        {
            await context.Inscripciones.AddRangeAsync(
                new Inscripcion { EventoId = eventos[0].Id, AsistenteId = asistentes[0].Id, FechaInscripcion = DateTime.Now.AddDays(-3), Estado = "Confirmada" },
                new Inscripcion { EventoId = eventos[1].Id, AsistenteId = asistentes[1].Id, FechaInscripcion = DateTime.Now.AddDays(-2), Estado = "Confirmada" },
                new Inscripcion { EventoId = eventos[2].Id, AsistenteId = asistentes[2].Id, FechaInscripcion = DateTime.Now.AddDays(-1), Estado = "Confirmada" });
            await context.SaveChangesAsync();
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}");

app.Run();
