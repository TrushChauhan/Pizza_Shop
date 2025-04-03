using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface ISectionAndTableService
{
    Task<List<SectionViewModel>> GetSectionsAsync();
    Task AddSectionAsync(SectionViewModel sectionViewModel);
    Task DeleteSectionAsync(int id);
}
