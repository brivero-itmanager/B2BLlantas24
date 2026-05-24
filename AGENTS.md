# AGENTS.md — Reglas operativas para agentes en ITManager

Objetivo: evolucionar ITManager sin romper DDD, separación por capas ni contratos públicos, manteniendo cambios pequeños, testeables y trazables.

## 1) Mapa del repositorio

La solución `ITManager.slnx` contiene:

- `ITManager.Domain`: núcleo de negocio.
- `ITManager.Application`: casos de uso (CQRS simple: Commands/Queries + Handlers).
- `ITManager.Infrastructure`: implementación técnica (repositorios, DbContext, integraciones).
- `ITManager.Api`: capa HTTP y composición de dependencias.
- `ITManager.Web`: cliente Blazor WASM.

## 2) Regla de dependencias (obligatoria)

Permitidas:

- `Api -> Application`
- `Api -> Infrastructure`
- `Application -> Domain`
- `Infrastructure -> Application` (para implementar contratos)

Prohibidas:

- `Domain -> *` (cualquier capa externa)
- `Application -> Infrastructure`
- `Web -> Domain`
- `Web -> Infrastructure`

Si una tarea requiere romper estas reglas: **detenerse, documentar y esperar confirmación**.

## 3) Arquitecturas y patrones a usar (estándar del repo)

### 3.1 Arquitectura base
- Estilo: **Clean Architecture por capas** + principios de **DDD táctico**.
- Organización por caso de uso dentro de `Application`.
- Composition Root en `Api/Program.cs`.

### 3.2 Patrón de casos de uso
- Usar **CQRS simple**:
  - `Commands/<Feature>/<Feature>Command.cs`
  - `Commands/<Feature>/<Feature>CommandHandler.cs`
  - `Queries/<Feature>/<Feature>Query.cs`
  - `Queries/<Feature>/<Feature>QueryHandler.cs`
  - `Queries/<Feature>/<Feature>Dto.cs` (si aplica)
- Regla: handlers con una sola responsabilidad y lógica de orquestación (no lógica técnica).

### 3.3 Dominio
- Entidades con invariantes y comportamiento mínimo necesario.
- Evitar modelos anémicos cuando se agreguen reglas de negocio reales.
- Contratos de repositorio definidos por necesidad de negocio, no por tecnología.

### 3.4 Persistencia
- En desarrollo/demo puede usarse repositorio in-memory.
- En producción, preferir implementación EF Core en `Infrastructure`.
- No filtrar ni mapear DTOs en `Domain`.
- **Nota:** El paquete `Microsoft.EntityFrameworkCore.Design` debe estar instalado en el proyecto `ITManager.Api` (startup project), no en `ITManager.Infrastructure`. Sin este paquete los comandos `dotnet ef` fallan con un error de referencia faltante.

### 3.5 API
- Controladores delgados:
  - parsean request,
  - llaman handler,
  - devuelven status code correcto.
- No duplicar lógica de negocio en controllers.
- Mantener backward compatibility de rutas/contratos salvo requerimiento explícito.

### 3.6 Web (Blazor)
- UI consume solo endpoints HTTP.
- Modelos de UI separados de entidades de dominio.
- Validaciones de formulario en UI son complementarias (no sustituyen validación de aplicación/dominio).

### 3.7 DI y composición
- Registrar en `Api`:
  - contratos + implementaciones,
  - handlers por caso de uso.
- Evitar service locator y singletons con estado mutable de negocio.

## 4) Responsabilidades por capa (estrictas)

### Domain
Sí:
- Entidades, Value Objects, reglas/invariantes, interfaces de dominio.

No:
- EF Core, SQL, HTTP, controllers, DTOs de API/Web, appsettings.

### Application
Sí:
- Casos de uso, flujo de negocio, coordinación de repositorios/servicios de dominio, DTOs de aplicación.

No:
- acceso técnico concreto a BD,
- conocimiento de transporte HTTP,
- dependencias a Infrastructure.

### Infrastructure
Sí:
- Implementaciones técnicas de contratos (`IRepository`, DbContext, adapters).

No:
- introducir reglas de negocio nuevas.

### Presentation (Api/Web)
Sí:
- requests/responses, serialización, validación de borde, render UI.

No:
- decisiones de negocio centrales.

## 5) Convenciones de desarrollo

1. **Feature first**: crear/ajustar caso de uso antes de refactors.
2. **Cambios mínimos**: tocar solo capas necesarias.
3. **No mezcla en commits**: feature + refactor amplio + style-only.
4. **Naming consistente**:
   - `*Command`, `*CommandHandler`
   - `*Query`, `*QueryHandler`
   - `*Dto`
5. **Async first**: operaciones de IO con `Task`/`await`.
6. **Validación por capas**:
   - UI/API: formato y campos obligatorios.
   - Application/Domain: reglas de negocio.
7. **Errores**:
   - no ocultar excepciones,
   - traducir a respuestas HTTP claras en API.
8. **Mapeos explícitos**: Entity ↔ DTO en Application (o mapping profile dedicado si crece).

## 6) Reglas de evolución (cuando el proyecto crezca)

- Si se agrega complejidad:
  - evaluar `MediatR` para pipeline behaviors (validación/logging),
  - agregar `FluentValidation` en Application,
  - agregar `Result<T>`/errores de negocio tipados.
- Mantener compatibilidad de endpoints existentes salvo cambio acordado.
- Extraer interfaces por caso de uso, no por “anticipación”.

## 7) Testing y no-regresión

Agregar/actualizar tests cuando:
- cambia regla de dominio,
- se crea/modifica un caso de uso,
- se corrige bug.

Prioridad de pruebas:
1. Unit tests de Domain (invariantes y comportamiento).
2. Unit tests de Application handlers (dobles de repositorio).
3. Integración de API para endpoints críticos.
4. Si aplica, pruebas básicas de UI (bUnit).

Si no hay proyecto de tests, reportar brecha y proponer estructura:
- `ITManager.Domain.Tests`
- `ITManager.Application.Tests`
- `ITManager.Api.IntegrationTests`

## 8) Comandos estándar

**Requisito previo:** `dotnet-ef` debe estar instalado como tool global. Si los comandos `dotnet ef` no son reconocidos, ejecutar:
```bash
dotnet tool install --global dotnet-ef --version 8.0.24
```
Cerrar y reabrir la terminal después de instalarlo.

Desde raíz:

```bash
dotnet restore ITManager.slnx
dotnet build ITManager.slnx
dotnet test ITManager.slnx
```