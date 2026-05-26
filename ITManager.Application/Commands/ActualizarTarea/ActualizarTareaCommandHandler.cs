using ITManager.Domain.Interfaces;

namespace ITManager.Application.Commands.ActualizarTarea
{
    public class ActualizarTareaCommandHandler(ITareaRepository repository)
    {
        public async Task HandleAsync(ActualizarTareaCommand command)
        {
            var tarea = await repository.GetByIdAsync(command.Id);
            if (tarea is null)
            {
                throw new KeyNotFoundException($"No se encontró la tarea con Id {command.Id}.");
            }

            if (command.Status == "sent")
            {
                tarea.MarcarComoEnviada();
            }
            else if (command.Status == "failed")
            {
                tarea.MarcarComoFallida(command.LastError ?? string.Empty);
            }

            tarea.IncrementarIntento();

            await repository.UpdateAsync(tarea);
        }
    }
}
