namespace ITManager.Application.Commands.ActualizarTarea
{
    public record ActualizarTareaCommand(long Id, string Status, string? LastError);
}
