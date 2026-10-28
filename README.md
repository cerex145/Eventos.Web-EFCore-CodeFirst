# Eventos Web

Aplicación ASP.NET Core MVC para gestionar eventos, asistentes e inscripciones mediante Entity Framework Core Code First y SQL Server.

## Requisitos

- .NET 8 SDK
- SQL Server
- `dotnet-ef` 8

## Configuración

1. Ajustar `EventosConnection` en `appsettings.json` si el servidor SQL usa otra instancia o credenciales.
2. Restaurar y aplicar las migraciones:

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

La aplicación aplica las migraciones al iniciar y carga datos iniciales solo cuando las tablas de categorías y asistentes están vacías.

Para una demostración sin SQL Server puede ejecutarse temporalmente con `UseInMemory=true`; la configuración normal continúa usando SQL Server y las migraciones incluidas.

## Migraciones incluidas

1. `InitialCreate`
2. `AgregarModalidadEvento`
3. `AgregarTelefonoAsistente`

## Funcionalidad

- Eventos con filtro por categoría, cupos disponibles, modalidad y eliminación lógica.
- Registro de asistentes con validación de DNI y correo únicos.
- Inscripciones con validación de fecha, duplicidad y aforo; cancelación lógica por estado.
- Repositorios asíncronos con LINQ, `AsNoTracking` y patrón Post/Redirect/Get.
