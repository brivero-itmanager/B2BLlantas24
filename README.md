# ITManager — Integration Hub SAP B1 ↔ WooCommerce

Plataforma de integración bidireccional entre SAP Business One y WooCommerce. Construida sobre Clean Architecture, CQRS simple, EF Core con SQL Server, Azure Service Bus y Blazor WASM como cliente de monitoreo.

## Estructura del proyecto

- `ITManager.Domain`: entidades, interfaces y contratos de dominio.
- `ITManager.Application`: casos de uso con CQRS simple (Commands/Queries + Handlers).
- `ITManager.Infrastructure`: repositorios EF Core, DbContext y configuraciones técnicas.
- `ITManager.Api`: controladores HTTP, webhooks de WooCommerce y composición de dependencias.
- `ITManager.Web`: cliente Blazor WebAssembly — tablero de monitoreo del Integration Hub.
- `ITManager.Worker`: proceso en background — polling de tareas, publicación en Service Bus, consumo y llamadas a WooCommerce.

## Requisitos previos

- .NET 8 SDK
- SQL Server (local o remoto)
- Azure Service Bus namespace configurado
- WooCommerce con token de API y webhooks configurados
- `dotnet-ef` instalado como tool global:

```bash
dotnet tool install --global dotnet-ef --version 8.0.24
```

## Configuración inicial

### Paso 1 — Cadena de conexión (API e Infrastructure)

Editar `ITManager.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=ITManagerDb;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;"
}
```

### Paso 2 — Configuración del Worker

Editar `ITManager.Worker/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR;Database=ITManagerDb;..."
},
"ServiceBus": {
  "ConnectionString": "TU_CONNECTION_STRING_SERVICE_BUS",
  "ToWooCommerceQueue": "itmanager-to-woocommerce",
  "FromWooCommerceQueue": "itmanager-from-woocommerce"
},
"WooCommerce": {
  "BaseUrl": "https://TU_WOOCOMMERCE/wp-json/wc/v3",
  "ConsumerKey": "TU_CONSUMER_KEY",
  "ConsumerSecret": "TU_CONSUMER_SECRET",
  "TimeoutSeconds": 10
},
"Worker": {
  "PollingIntervalSeconds": 10,
  "BatchSize": 50
}
```

### Paso 3 — Aplicar migración

Desde la raíz de la solución:

```bash
dotnet ef database update --project ITManager.Infrastructure --startup-project ITManager.Api
```

### Paso 4 — Levantar la API

```bash
dotnet run --project ITManager.Api
```

API disponible en `https://localhost:7232`. Swagger en `https://localhost:7232/swagger`.

### Paso 5 — Levantar el Worker

```bash
dotnet run --project ITManager.Worker
```

### Paso 6 — Levantar el cliente Web

```bash
dotnet run --project ITManager.Web
```

Cliente disponible en `https://localhost:7065`.

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

## Integration Hub — Flujos principales

### SAP B1 → WooCommerce
```
SAP B1 → trigger SQL → Tabla Tareas (pending)
→ TareaPollingWorker → Azure Service Bus
→ TareaConsumerWorker → Transformación → WooCommerce API
→ Tarea actualizada (sent / failed / superseded)
```

### WooCommerce → SAP B1
```
WooCommerce → webhook → POST /api/webhooks/woocommerce
→ Tarea nueva en DB → Worker → SAP B1
```

## Tipos de tarea soportados

| Tarea | Dirección | Descripción |
|-------|-----------|-------------|
| `UPDATE_STOCK` | SAP → WooCommerce | Actualiza inventario por UEN+almacén+SKU |
| `PRICE_CHANGED` | SAP → WooCommerce | Actualiza precio de artículo |
| `ITEM_PUBLISHED` | SAP → WooCommerce | Publica artículo |
| `ITEM_DRAFTED` | SAP → WooCommerce | Convierte artículo a borrador |
| `ORDER_APPROVED` | SAP → WooCommerce | Notifica aprobación de pedido |
| `ORDER_INVOICED` | SAP → WooCommerce | Notifica facturación |
| `ORDER_SHIPPED` | SAP → WooCommerce | Notifica remisión |
| `HEALTH_CHECK` | SAP → WooCommerce | Validación de conexión desde tablero |

## Statuses de Tarea

| Status | Significado |
|--------|-------------|
| `pending` | Esperando procesamiento |
| `in_progress` | Siendo procesada actualmente |
| `sent` | Enviada exitosamente |
| `failed` | Falló definitivamente — requiere revisión |
| `superseded` | Reemplazada por tarea más reciente del mismo SKU+UEN+almacén |

## Plan de fases del Worker

| Fase | Qué construye | Estado |
|------|--------------|--------|
| 1 | Proyecto Worker + TareaLog + Serilog + polling básico | Pendiente |
| 2 | Azure Service Bus + flujo completo UPDATE_STOCK | Pendiente |
| 3 | Resiliencia: Circuit Breaker + Polly + reintentos + DLQ | Pendiente |
| 4 | Tablero de monitoreo + alertas + Validar Conexión | Pendiente |
| 5 | Webhooks WooCommerce → SAP + tipos de tarea adicionales | Pendiente |

## Documentación

- `AGENTS.md` — reglas operativas, convenciones de arquitectura, decisiones del Integration Hub y guía para agregar features.