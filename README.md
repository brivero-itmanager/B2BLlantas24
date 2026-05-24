# ITManager Template

Template base de arquitectura limpia para proyectos .NET 8 internos. Incluye Clean Architecture, CQRS simple, EF Core con SQL Server, ErrorOr para manejo de errores y Blazor WASM como cliente.

## Estructura del proyecto

- `ITManager.Domain`: entidades, interfaces y contratos de dominio.
- `ITManager.Application`: casos de uso con CQRS simple (Commands/Queries + Handlers).
- `ITManager.Infrastructure`: repositorios EF Core, DbContext y configuraciones técnicas.
- `ITManager.Api`: controladores HTTP y composición de dependencias.
- `ITManager.Web`: cliente Blazor WebAssembly.

## Requisitos previos

- .NET 8 SDK
- SQL Server (local o remoto)
- `dotnet-ef` instalado como tool global:

```bash
dotnet tool install --global dotnet-ef --version 8.0.24
```

## Configuración inicial

### Paso 1 — Cadena de conexión

Editar `ITManager.Api/appsettings.json` y configurar `ConnectionStrings:DefaultConnection` con los datos del servidor SQL:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=ITManagerDb;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
}
```

### Paso 2 — Aplicar migración

Desde la raíz de la solución:

```bash
dotnet ef database update --project ITManager.Infrastructure --startup-project ITManager.Api
```

### Paso 3 — Levantar la API

```bash
dotnet run --project ITManager.Api
```

La API queda disponible en `https://localhost:7232`. Swagger en `https://localhost:7232/swagger`.

### Paso 4 — Levantar el cliente Web

En otra terminal:

```bash
dotnet run --project ITManager.Web
```

El cliente queda disponible en `https://localhost:7065`.

## Comandos útiles

```bash
# Restaurar paquetes
dotnet restore ITManager.slnx

# Compilar solución completa
dotnet build ITManager.slnx

# Crear nueva migración
dotnet ef migrations add NombreMigracion --project ITManager.Infrastructure --startup-project ITManager.Api

# Aplicar migraciones pendientes
dotnet ef database update --project ITManager.Infrastructure --startup-project ITManager.Api

# Revertir última migración
dotnet ef migrations remove --project ITManager.Infrastructure --startup-project ITManager.Api
```

## Cómo usar este template

Este repositorio es una base de arranque. Para un proyecto nuevo:

1. Clonar o hacer fork del repositorio.
2. Renombrar la solución y los proyectos reemplazando `ITManager` por el nombre del nuevo proyecto.
3. Eliminar o adaptar el feature `Sample` según las necesidades del proyecto.
4. Seguir las convenciones documentadas en `AGENTS.md`.

## Documentación

- `AGENTS.md` — reglas operativas, convenciones de arquitectura y guía para agregar features.
