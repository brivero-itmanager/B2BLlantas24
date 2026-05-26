using ITManager.Application.Commands.ActualizarTarea;
using ITManager.Application.Commands.CrearTarea;
using ITManager.Application.Queries.GetTareaById;
using ITManager.Application.Queries.GetTareas;
using ITManager.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ITManager.Api.Controllers
{
    [ApiController]
    [Route("api/tareas")]
    public class TareasController(
        CrearTareaCommandHandler crearTareaCommandHandler,
        ActualizarTareaCommandHandler actualizarTareaCommandHandler,
        GetTareasQueryHandler getTareasQueryHandler,
        GetTareaByIdQueryHandler getTareaByIdQueryHandler) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearTareaCommand command)
        {
            var id = await crearTareaCommandHandler.HandleAsync(command);
            return Created($"/api/tareas/{id}", new { id });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] ActualizarTareaRequest request)
        {
            try
            {
                await actualizarTareaCommandHandler.HandleAsync(
                    new ActualizarTareaCommand(id, request.Status, request.LastError));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<List<TareaDto>>> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? uen,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 200)
        {
            var result = await getTareasQueryHandler.HandleAsync(
                new GetTareasQuery(status, uen, desde, hasta, page, pageSize));

            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<TareaDto>> GetById(long id)
        {
            try
            {
                var result = await getTareaByIdQueryHandler.HandleAsync(new GetTareaByIdQuery (id));
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
