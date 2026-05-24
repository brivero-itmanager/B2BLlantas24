using ITManager.Web.Models;

namespace ITManager.Web.Services;

public interface ISampleService
{
    Task<List<SampleItemModel>> GetAllAsync();
    Task<SampleItemModel?> GetByIdAsync(Guid id);
    Task<Guid?> CreateAsync(CreateSampleModel model);
    Task<bool> UpdateAsync(Guid id, EditSampleModel model);
    Task<bool> DeleteAsync(Guid id);
}
