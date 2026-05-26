namespace ITManager.Web.Models;

public sealed class TareaItemModel
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Uen { get; set; } = string.Empty;
    public string NombreTarea { get; set; } = string.Empty;
    public string Json { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Attempts { get; set; }
    public string? LastError { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
