using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations;

public class SectionAndTableService : ISectionAndTableService
{
    private readonly ISectionAndTableRepository _sectionAndTablerepo;
    private readonly MappingService _mappingService;
    public SectionAndTableService(ISectionAndTableRepository sectionAndTableRepository, MappingService mappingService)
    {
        _sectionAndTablerepo = sectionAndTableRepository;
        _mappingService = mappingService;
    }
    public async Task<List<SectionViewModel>> GetSectionsAsync()
    {
        var sections = await _sectionAndTablerepo.GetSectionsAsync();
        return sections.Select(s => _mappingService.MapToViewSectionModel(s)).ToList();
    }
}
