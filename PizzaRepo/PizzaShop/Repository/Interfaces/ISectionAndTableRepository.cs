using Entity.Models;
using Entity.ViewModel;

namespace Repository.Interfaces;

public interface ISectionAndTableRepository
{
    Task<List<Section>> GetSectionsAsync();
}
