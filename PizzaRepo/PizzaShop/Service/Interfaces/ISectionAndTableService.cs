using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface ISectionAndTableService
{
    Task<List<SectionViewModel>> GetSectionsAsync();
    Task AddSectionAsync(SectionViewModel sectionViewModel);
    Task DeleteSectionAsync(int id);
    Task<SectionViewModel>GetSectionByIdAsync(int id);
    Task<bool> UpdateSectionAsync(SectionViewModel model);
    Task<(List<DinetableViewModel> tables, int totalItems)> GetTablesBySectionAsync(int sectionId, int page, int pageSize, string searchTerm);
    Task AddTableAsync( DinetableViewModel table);
    Task<bool> UpdateTableAsync(DinetableViewModel model);
    Task<DinetableViewModel> GetTableByIdAsync(int id);
    Task DeleteTableAsync(int tableId);
    Task MassDeleteTablesAsync(List<int> tableIds);

}
