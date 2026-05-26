namespace ITManager.Application.Queries.GetTareas
{
    public record GetTareasQuery(
        string? Status = null,
        string? Uen = null,
        DateTime? Desde = null,
        DateTime? Hasta = null,
        int Page = 1,
        int PageSize = 200);
}
