namespace ITManager.Application.Queries.GetTareas
{
    public record GetTareasQuery
    {
        public string? Status { get; init; }
        public string? Uen { get; init; }
        public DateTime? Desde { get; init; }
        public DateTime? Hasta { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 200;
    }
}
