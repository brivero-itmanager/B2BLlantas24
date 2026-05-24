using System.Net.Http.Json;
using System.Text.Json;
using ITManager.Web.Models;

namespace ITManager.Web.Services;

public sealed class SampleService(HttpClient httpClient) : ISampleService
{
    public async Task<List<SampleItemModel>> GetAllAsync()
    {
        var result = await httpClient.GetFromJsonAsync<List<SampleItemModel>>("api/sample");
        return result ?? [];
    }

    public async Task<SampleItemModel?> GetByIdAsync(Guid id)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<SampleItemModel>($"api/sample/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<Guid?> CreateAsync(CreateSampleModel model)
    {
        var response = await httpClient.PostAsJsonAsync("api/sample", model);
        if (!response.IsSuccessStatusCode) return null;

        try
        {
            var body = await response.Content.ReadFromJsonAsync<JsonElement>();
            if (body.TryGetProperty("id", out var idProperty) &&
                Guid.TryParse(idProperty.GetString(), out var guid))
                return guid;
        }
        catch { return null; }

        return null;
    }

    public async Task<bool> UpdateAsync(Guid id, EditSampleModel model)
    {
        try
        {
            var response = await httpClient.PutAsJsonAsync($"api/sample/{id}", model);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var response = await httpClient.DeleteAsync($"api/sample/{id}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
