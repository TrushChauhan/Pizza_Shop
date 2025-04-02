using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface ISectionAndTableService
{
    Task<List<SectionViewModel>> GetSectionsAsync();
}
