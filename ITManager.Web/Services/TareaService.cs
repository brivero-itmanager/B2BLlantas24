using System.Net.Http.Json;
using ITManager.Web.Models;

namespace ITManager.Web.Services;

public sealed class TareaService(HttpClient httpClient) : ITareaService
{
    public async Task<List<TareaItemModel>> GetAllAsync(
        string? status,
        string? uen,
        DateTime? desde,
        DateTime? hasta,
        int page = 1,
        int pageSize = 200)
    {
        var queryParams = new List<string>();

        if (!string.IsNullOrWhiteSpace(status))
            queryParams.Add($"status={Uri.EscapeDataString(status)}");

        if (!string.IsNullOrWhiteSpace(uen))
            queryParams.Add($"uen={Uri.EscapeDataString(uen)}");

        if (desde.HasValue)
            queryParams.Add($"desde={desde.Value:yyyy-MM-dd}");

        if (hasta.HasValue)
            queryParams.Add($"hasta={hasta.Value:yyyy-MM-dd}");

        queryParams.Add($"page={page}");
        queryParams.Add($"pageSize={pageSize}");

        var query = string.Join("&", queryParams);
        var url = queryParams.Count > 0 ? $"api/tareas?{query}" : "api/tareas";

        var result = await httpClient.GetFromJsonAsync<List<TareaItemModel>>(url);
        return result ?? [];
    }
}
