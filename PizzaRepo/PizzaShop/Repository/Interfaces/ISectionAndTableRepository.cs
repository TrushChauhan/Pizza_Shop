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
}
