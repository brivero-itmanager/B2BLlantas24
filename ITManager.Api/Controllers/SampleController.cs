using ITManager.Application.Commands.CreateSample;
using ITManager.Application.Commands.DeleteSample;
using ITManager.Application.Commands.UpdateSample;
using ITManager.Application.Queries.GetSampleById;
using ITManager.Application.Queries.GetSamples;
using Microsoft.AspNetCore.Mvc;

namespace ITManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly GetSamplesQueryHandler _getSamplesHandler;
        private readonly GetSampleByIdQueryHandler _getSampleByIdHandler;
        private readonly CreateSampleCommandHandler _createSampleHandler;
        private readonly UpdateSampleCommandHandler _updateSampleHandler;
        private readonly DeleteSampleCommandHandler _deleteSampleHandler;

        public SampleController(
            GetSamplesQueryHandler getSamplesHandler,
            GetSampleByIdQueryHandler getSampleByIdHandler,
            CreateSampleCommandHandler createSampleHandler,
            UpdateSampleCommandHandler updateSampleHandler,
            DeleteSampleCommandHandler deleteSampleHandler)
        {
            _getSamplesHandler = getSamplesHandler;
            _getSampleByIdHandler = getSampleByIdHandler;
            _createSampleHandler = createSampleHandler;
            _updateSampleHandler = updateSampleHandler;
            _deleteSampleHandler = deleteSampleHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _getSamplesHandler.HandleAsync(new GetSamplesQuery());
            if (result.IsError)
            {
                return Problem();
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _getSampleByIdHandler.HandleAsync(new GetSampleByIdQuery { Id = id });
            if (result.IsError)
            {
                return result.FirstError.Type == ErrorOr.ErrorType.NotFound ? NotFound() : Problem();
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSampleCommand command)
        {
            var result = await _createSampleHandler.HandleAsync(command);
            if (result.IsError)
            {
                return Problem();
            }

            return Created($"/api/sample/{result.Value}", new { id = result.Value });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSampleCommand command)
        {
            command.Id = id;
            var result = await _updateSampleHandler.HandleAsync(command);
            if (result.IsError)
            {
                return result.FirstError.Type == ErrorOr.ErrorType.NotFound ? NotFound() : Problem();
            }

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _deleteSampleHandler.HandleAsync(new DeleteSampleCommand { Id = id });
            if (result.IsError)
            {
                return result.FirstError.Type == ErrorOr.ErrorType.NotFound ? NotFound() : Problem();
            }

            return NoContent();
        }
    }
}
