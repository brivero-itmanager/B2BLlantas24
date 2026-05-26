using ITManager.Domain.Entities;
using ITManager.Domain.Interfaces;

namespace ITManager.Application.Commands.CrearTarea
{
    public class CrearTareaCommandHandler(ITareaRepository repository)
    {
        public async Task<long> HandleAsync(CrearTareaCommand command)
        {
            var tarea = new Tarea(command.Uen, command.NombreTarea, command.Json);
            await repository.AddAsync(tarea);
            return tarea.Id;
        }
    }
}
