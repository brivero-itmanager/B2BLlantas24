using ITManager.Web.Models;

namespace ITManager.Web.Services;

public interface ITareaService
{
    Task<List<TareaItemModel>> GetAllAsync(
        string? status,
        string? uen,
        DateTime? desde,
        DateTime? hasta,
        int page = 1,
        int pageSize = 200);
}
