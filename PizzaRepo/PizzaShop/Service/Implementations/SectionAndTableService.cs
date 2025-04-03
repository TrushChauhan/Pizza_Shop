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
        List<SectionViewModel> sectionViews = new();
        foreach (Section section in sections)
        {   
            sectionViews.Add( await _mappingService.MapToViewSectionModel(section));
        } 
        return sectionViews;
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
    public async Task<SectionViewModel>GetSectionByIdAsync(int id){
        Section section=await _sectionAndTablerepo.GetSectionByIdAsync(id);
        return await _mappingService.MapToViewSectionModel(section);
    }
    public async Task<bool> UpdateSectionAsync(SectionViewModel model)
    {
        var section = new Section
        {
            Sectionid=model.Sectionid,
            Sectionname = model.Sectionname,
            Modifieddate = DateTime.Now,
            Description=model.Description,
            Createddate=model.Createddate
        };
        return await _sectionAndTablerepo.UpdateSectionAsync(section);
    }
}
