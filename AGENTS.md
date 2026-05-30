# AGENTS.md — Reglas operativas para agentes en ITManager

Objetivo: evolucionar ITManager sin romper DDD, separación por capas ni contratos públicos, manteniendo cambios pequeños, testeables y trazables.

## 1) Mapa del repositorio

La solución `ITManager.slnx` contiene:

- `ITManager.Domain`: núcleo de negocio.
- `ITManager.Application`: casos de uso (CQRS simple: Commands/Queries + Handlers).
- `ITManager.Infrastructure`: implementación técnica (repositorios, DbContext, integraciones).
- `ITManager.Api`: capa HTTP y composición de dependencias.
- `ITManager.Web`: cliente Blazor WASM.
- `ITManager.Worker`: proceso en background — Integration Hub bidireccional SAP B1 ↔ WooCommerce.

## 2) Regla de dependencias (obligatoria)

Permitidas:

- `Api -> Application`
- `Api -> Infrastructure`
- `Application -> Domain`
- `Infrastructure -> Application` (para implementar contratos)
- `Worker -> Application`
- `Worker -> Infrastructure`

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
- Composition Root en `Api/Program.cs` y `Worker/Program.cs`.

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
- Usar **Aggregate Root** cuando una entidad posee a otra (ejemplo: `Tarea` → `TareaLog`).
- Usar **Domain Services** cuando dos entidades independientes comparten una regla de negocio.

### 3.4 Persistencia
- En desarrollo/demo puede usarse repositorio in-memory.
- En producción, preferir implementación EF Core en `Infrastructure`.
- No filtrar ni mapear DTOs en `Domain`.
- **Nota:** El paquete `Microsoft.EntityFrameworkCore.Design` debe estar instalado en el proyecto `ITManager.Api` (startup project), no en `ITManager.Infrastructure`. Sin este paquete los comandos `dotnet ef` fallan con un error de referencia faltante.
- La carpeta de persistencia en Infrastructure se llama `Persistance` (con esa ortografía) — mantener consistencia.

### 3.5 API
- Controladores delgados:
  - parsean request,
  - llaman handler,
  - devuelven status code correcto.
- No duplicar lógica de negocio en controllers.
- Mantener backward compatibility de rutas/contratos salvo requerimiento explícito.
- Request models de API (`*Request.cs`) en `ITManager.Api/Models/` — separados de los Commands de Application.

### 3.6 Web (Blazor)
- UI consume solo endpoints HTTP.
- Modelos de UI separados de entidades de dominio.
- Validaciones de formulario en UI son complementarias (no sustituyen validación de aplicación/dominio).

### 3.7 DI y composición
- Registrar en `Api` y `Worker`:
  - contratos + implementaciones,
  - handlers por caso de uso.
- Evitar service locator y singletons con estado mutable de negocio.
- Excepción: `CircuitBreakerService` en Worker puede ser singleton (estado en memoria, instancia única).

### 3.8 Worker — Integration Hub
Ver sección 10 para las reglas completas del Worker.

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

### Worker
Sí:
- Polling de tareas, publicación en Service Bus, consumo de Service Bus, llamadas a WooCommerce, transformación de payloads, Circuit Breaker, resiliencia.

No:
- reglas de negocio de dominio (viven en Domain),
- lógica de persistencia directa (usa repositorios de Infrastructure),
- exposición de endpoints HTTP.

## 5) Convenciones de desarrollo

1. **Feature first**: crear/ajustar caso de uso antes de refactors.
2. **Cambios mínimos**: tocar solo capas necesarias.
3. **No mezcla en commits**: feature + refactor amplio + style-only.
4. **Naming consistente**:
   - `*Command`, `*CommandHandler`
   - `*Query`, `*QueryHandler`
   - `*Dto`
   - `*Transformer` (Worker)
   - `*Worker` (BackgroundService)
5. **Async first**: operaciones de IO con `Task`/`await`.
6. **Validación por capas**:
   - UI/API: formato y campos obligatorios.
   - Application/Domain: reglas de negocio.
7. **Errores**:
   - no ocultar excepciones,
   - traducir a respuestas HTTP claras en API.
8. **Mapeos explícitos**: Entity ↔ DTO en Application (o mapping profile dedicado si crece).
9. Fechas: usar DateTime.Now en todo el sistema — la aplicación opera exclusivamente en zona horaria CDMX (UTC-6). No usar DateTime.UtcNow.

## 6) Reglas de evolución (cuando el proyecto crezca)

- Si se agrega complejidad:
  - evaluar `MediatR` para pipeline behaviors (validación/logging),
  - agregar `FluentValidation` en Application,
  - agregar `Result<T>`/errores de negocio tipados.
- Mantener compatibilidad de endpoints existentes salvo cambio acordado.
- Extraer interfaces por caso de uso, no por "anticipación".

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
dotnet run --project ITManager.Worker
```

## 9) Formato de prompts para tareas

Todo prompt enviado a un agente (Codex u otro) debe seguir esta estructura fija.
No omitir secciones aunque parezcan vacías — escribir "N/A" si no aplica.

---

**PROMPT XX — [Nombre corto de la tarea]**

### Contexto
Estado actual del proyecto relevante para esta tarea: qué archivos existen,
qué problema hay, cuál es el punto de partida. Ser específico con rutas y nombres.

### Objetivo de la tarea
Una sola oración que delimite el alcance. Qué se quiere lograr y nada más.

### Cambios requeridos
Numerado por archivo. Para cada uno:
- Ruta exacta del archivo (crear o modificar)
- Qué hacer
- Snippet de código solo cuando haya ambigüedad en firma o comportamiento esperado

### Restricciones
Lista de lo que NO se debe tocar: capas, archivos, paquetes, patrones.
Incluir siempre:
- Qué proyectos están fuera de scope
- Que no se ejecuten comandos de terminal
- Que no se agreguen paquetes NuGet salvo indicación explícita

### Retro esperada al terminar
Al finalizar, el agente debe responder con:
1. ¿Qué se hizo? — archivos creados y modificados con descripción de cada cambio
2. ¿Qué decisiones se tomaron? — ambigüedades encontradas y cómo se resolvieron
3. ¿Qué quedó pendiente o con duda? — lo que no se pudo resolver o requiere confirmación
4. ¿Se encontró algo que contradice la arquitectura? — conflictos con las reglas del AGENTS.md

## 10) Integration Hub — Reglas y decisiones de arquitectura

El proyecto `ITManager.Worker` implementa un Integration Hub bidireccional entre SAP B1 y WooCommerce. Esta sección documenta las decisiones de diseño tomadas para este componente.

### 10.1 Propósito

La tabla `Tareas` es el corazón del hub. Todo lo que necesita sincronizarse entre SAP B1 y WooCommerce pasa por ella. El Worker la lee, transforma y envía a WooCommerce vía Azure Service Bus.

### 10.2 Flujos

**SAP B1 → WooCommerce**
```
SAP B1 → trigger SQL → Tabla Tareas (pending)
→ TareaPollingWorker → Azure Service Bus
→ TareaConsumerWorker → Transformación → WooCommerce API
→ Tarea actualizada (sent / failed / superseded)
```

**WooCommerce → SAP B1**
```
WooCommerce → webhook → ITManager.Api (POST /api/webhooks/woocommerce)
→ Tarea nueva en DB → Worker la procesa → SAP B1
```

### 10.3 Estructura del proyecto Worker

```
ITManager.Worker
├── Workers/
│   ├── TareaPollingWorker.cs      — polling DB cada N segundos, publica en Service Bus
│   └── TareaConsumerWorker.cs     — consume Service Bus, llama WooCommerce
├── Transformers/
│   ├── ITaskTransformer.cs        — contrato de transformación
│   └── UpdateStockTransformer.cs  — SAP JSON → WooCommerce payload (y más según fases)
├── Services/
│   ├── IWooCommerceService.cs     — contrato HTTP a WooCommerce
│   ├── WooCommerceService.cs      — implementación con autenticación por token
│   └── CircuitBreakerService.cs   — estado del Circuit Breaker en memoria
├── Models/
│   └── WooCommercePayload.cs      — modelos de request/response de WooCommerce
└── Program.cs
```

### 10.4 Statuses de Tarea

| Status | Significado |
|--------|-------------|
| `pending` | Esperando procesamiento |
| `in_progress` | Siendo procesada actualmente |
| `sent` | Enviada exitosamente a WooCommerce |
| `failed` | Falló definitivamente — requiere revisión |
| `superseded` | Reemplazada por una tarea más reciente del mismo SKU+UEN+almacén |

### 10.5 Deduplicación

Antes de publicar en Service Bus, el PollingWorker agrupa tareas `pending` por `UEN+almacén+SKU`. Por cada grupo conserva solo la más reciente y marca las demás como `superseded`. Esto garantiza que WooCommerce recibe el valor más actualizado, especialmente después de períodos de caída.

`superseded` no es un fallo — es comportamiento esperado y visible en el tablero para auditoría.

### Deduplicación en dos puntos

La deduplicación opera en dos momentos distintos:

**Punto 1 — PollingWorker (antes de publicar a Service Bus)**
Agrupa tareas `pending` por `DeduplicationKey` en el batch actual.
Conserva la más reciente, marca las demás como `superseded`.
Resuelve acumulación durante caída del Worker.

**Punto 2 — ConsumerWorker (antes de llamar WooCommerce)**
Antes de procesar un mensaje, verifica si existe una tarea más reciente
del mismo `DeduplicationKey` con status `in_progress` o `pending`.
Si existe → marca la tarea actual como `superseded`, descarta el mensaje.
Si no existe → llama WooCommerce.
Resuelve acumulación en Service Bus durante caída de WooCommerce.

Método requerido en `ITareaRepository`:
`Task<bool> ExisteVersionMasRecienteAsync(long tareaId, string deduplicationKey)`

### 10.6 Azure Service Bus — Configuración

- **Colas:**
  - `itmanager-to-woocommerce` — SAP B1 → WooCommerce
  - `itmanager-from-woocommerce` — WooCommerce → SAP B1
- **Sessions habilitadas** por `UEN+almacén+SKU` — garantiza orden FIFO por SKU.
- **MaxDeliveryCount:** 5 — después de 5 intentos el mensaje va a Dead Letter Queue.
- **Message TTL:** configurable según SLA del negocio.
- **Dead Letter Queue (DLQ):** mensajes fallidos definitivos — reprocessamiento manual desde tablero.

### 10.7 Circuit Breaker

El Circuit Breaker vive en memoria del Worker (`CircuitBreakerService`, singleton). Tiene dos dimensiones independientes:

**Dimensión 1 — Disponibilidad de WooCommerce (errores 500 / timeout)**

| Estado | Condición de entrada | Comportamiento |
|--------|---------------------|----------------|
| CLOSED | Operación normal | Procesa con normalidad |
| OPEN | 3+ fallos 500/timeout consecutivos | Pausa todo, inicia timer de 5 minutos |
| HALF-OPEN | Timer cumplido | Prueba una sola llamada |

- OPEN → CLOSED: llamada exitosa en HALF-OPEN.
- OPEN → OPEN: fallo en HALF-OPEN, resetea timer.
- Se recupera automáticamente después del timer.

**Dimensión 2 — Calidad de datos (errores 400 / 401)**

- 5 fallos 400/401 consecutivos del **mismo tipo de tarea** → pausa ese tipo específico + alerta.
- 5 fallos 400/401 consecutivos de **cualquier tipo** → circuito global abierto + alerta crítica.
- Errores 400/401 globales indican token expirado o WooCommerce reiniciándose.
- **No se recupera automáticamente** — requiere intervención manual desde el tablero.
- Los errores 400/401 **no** alimentan el Circuit Breaker de disponibilidad.

### 10.8 Reintentos y backoff exponencial

| Intento | Espera |
|---------|--------|
| 1 | 10 segundos |
| 2 | 30 segundos |
| 3 | 2 minutos |
| 4 | 10 minutos |
| 5 | Fallo definitivo → DLQ + tarea `failed` + alerta |

Librería: **Polly** — estándar de resiliencia en .NET. Aprobado para este proyecto.

### 10.9 Clasificación de errores

| Error | Causa probable | Comportamiento |
|-------|---------------|----------------|
| 500 / timeout | WooCommerce caído temporalmente | Reintenta con backoff; Circuit Breaker de disponibilidad |
| 429 | WooCommerce saturado | Reintenta con backoff |
| 400 | Payload inválido / problema de datos | Fallo inmediato, no reintenta; cuenta para Circuit Breaker de calidad |
| 401 | Token expirado o inválido | Fallo inmediato, no reintenta; cuenta para Circuit Breaker de calidad |
| 404 | Recurso no existe en WooCommerce | Fallo inmediato, no reintenta |

### 10.10 Proceso de recuperación después de caída prolongada

1. WooCommerce revive.
2. Equipo técnico presiona **"Validar Conexión"** en el tablero.
3. API crea tarea `HEALTH_CHECK` → Worker la procesa → WooCommerce responde → dispara webhook de confirmación.
4. Tablero espera webhook hasta **90 segundos**. Si no llega, muestra advertencia pero permite reset manual.
5. Equipo técnico presiona **"Resetear Circuit Breaker"** — habilitado tras validación exitosa.
6. PollingWorker despierta → ejecuta deduplicación → publica solo tareas únicas en Service Bus.
7. ConsumerWorker procesa en orden FIFO.

### 10.11 Tablero de monitoreo (Web)

Audiencia: equipo técnico únicamente.

Contenido:
- Total de tareas por status en tiempo real (pending, in_progress, sent, failed, superseded).
- Tareas procesadas en la última hora.
- Indicador de salud del sistema (verde / amarillo / rojo).
- Tabla de tareas con detalle: fechas, intentos, errores, logs por tarea.
- Estado del Circuit Breaker (CLOSED / OPEN / HALF-OPEN) con motivo.
- Botón "Validar Conexión" — health check real contra WooCommerce vía webhook.
- Botón "Resetear Circuit Breaker" — habilitado tras validación exitosa.
- Alertas: correo y/o WhatsApp API cuando hay fallos críticos o circuito abierto.

### 10.12 Plan de fases

| Fase | Qué construye |
|------|--------------|
| 1 | Proyecto Worker + estructura base + TareaLog como Aggregate + Serilog + polling básico |
| 2 | Azure Service Bus + flujo completo UPDATE_STOCK de punta a punta |
| 3 | Resiliencia: Circuit Breaker + Polly + reintentos + DLQ |
| 4 | Tablero de monitoreo + alertas + Validar Conexión + Reset Circuit Breaker |
| 5 | Webhooks WooCommerce → SAP + tipos de tarea adicionales |

### 10.13 Paquetes NuGet aprobados para Worker

- **Polly** — resiliencia, reintentos y Circuit Breaker.
- **Azure.Messaging.ServiceBus** — cliente de Azure Service Bus.
- **Serilog** + **Serilog.Sinks.Console** + **Serilog.Sinks.File** — logging estructurado.

