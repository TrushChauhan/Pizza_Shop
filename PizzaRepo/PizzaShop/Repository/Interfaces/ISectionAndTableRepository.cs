using Entity.Models;
using Entity.ViewModel;

namespace Repository.Interfaces;

public interface ISectionAndTableRepository
{
    Task<List<Section>> GetSectionsAsync();
    Task AddSectionAsync(Section section);
    Task DeleteSectionAsync(int id);
    Task<Section>GetSectionByIdAsync(int id);
    Task<bool> UpdateSectionAsync(Section section);
    Task<(List<DinetableViewModel> tables, int totalItems)> GetTablesBySectionAsync(int sectionId, int page, int pageSize, string searchTerm);
    Task AddTableAsync(Dinetable table);
    Task<Dinetable> GetTableByIdAsync(int id);
    Task<bool> UpdateTableAsync(Dinetable model);
    Task DeleteTableAsync(int tableId);
    Task MassDeleteTablesAsync(List<int> ids);
}
