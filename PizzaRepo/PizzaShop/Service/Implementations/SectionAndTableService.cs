using Entity.Models;
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
        List<Section> sections = await _sectionAndTablerepo.GetSectionsAsync();
        return sections.Select(s => _mappingService.MapToViewSectionModel(s)).ToList();
    }
    public async Task AddSectionAsync(SectionViewModel sectionViewModel){
        Section section = new Section{
            Sectionid=sectionViewModel.Sectionid,
            Sectionname=sectionViewModel.Sectionname,
            Description=sectionViewModel.Description,
            Createddate=DateTime.Now,
            Isdeleted=false
        };
        await _sectionAndTablerepo.AddSectionAsync(section);
    }
    public async Task DeleteSectionAsync(int id){
        await _sectionAndTablerepo.DeleteSectionAsync(id);
    }
}
