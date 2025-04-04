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
            sectionViews.Add(await _mappingService.MapToViewSectionModel(section));
        }
        return sectionViews;
    }
    public async Task AddSectionAsync(SectionViewModel sectionViewModel)
    {
        Section section = new Section
        {
            Sectionid = sectionViewModel.Sectionid,
            Sectionname = sectionViewModel.Sectionname,
            Description = sectionViewModel.Description,
            Createddate = DateTime.Now,
            Isdeleted = false
        };
        await _sectionAndTablerepo.AddSectionAsync(section);
    }
    public async Task DeleteSectionAsync(int id)
    {
        await _sectionAndTablerepo.DeleteSectionAsync(id);
    }
    public async Task<SectionViewModel> GetSectionByIdAsync(int id)
    {
        Section section = await _sectionAndTablerepo.GetSectionByIdAsync(id);
        return await _mappingService.MapToViewSectionModel(section);
    }
    public async Task<bool> UpdateSectionAsync(SectionViewModel model)
    {
        var section = new Section
        {
            Sectionid = model.Sectionid,
            Sectionname = model.Sectionname,
            Modifieddate = DateTime.Now,
            Description = model.Description,
            Createddate = model.Createddate
        };
        return await _sectionAndTablerepo.UpdateSectionAsync(section);
    }
    public async Task<(List<DinetableViewModel> tables, int totalItems)> GetTablesBySectionAsync(int sectionId, int page, int pageSize, string searchTerm)
    {
        return await _sectionAndTablerepo.GetTablesBySectionAsync(sectionId, page, pageSize, searchTerm);
    }
    public async Task AddTableAsync(DinetableViewModel tablemodel)
    {
        Dinetable table = new Dinetable
        {
            Sectionid = tablemodel.Sectionid,
            Tablename = tablemodel.Tablename,
            Status = tablemodel.Status,
            Capacity = tablemodel.Capacity,
            Isdeleted = false,
            Createddate = DateTime.Now,
            Modifieddate = DateTime.Now
        };
        await _sectionAndTablerepo.AddTableAsync(table);
    }
    public async Task<DinetableViewModel> GetTableByIdAsync(int id)
    {
        Dinetable table = await _sectionAndTablerepo.GetTableByIdAsync(id);
        return await _mappingService.MapToViewTableModel(table);
    }

    public async Task<bool> UpdateTableAsync(DinetableViewModel model)
    {
        var table = new Dinetable
        {
            Tableid = model.Tableid,
            Tablename = model.Tablename,
            Capacity = model.Capacity,
            Status = model.Status,
            Sectionid = model.Sectionid,
            Modifieddate = DateTime.Now
        };
        return await _sectionAndTablerepo.UpdateTableAsync(table);
    }
    public async Task DeleteTableAsync(int tableId)
    {
        await _sectionAndTablerepo.DeleteTableAsync(tableId);
    }
    public async Task MassDeleteTablesAsync(List<int> tableIds)
    {
        await _sectionAndTablerepo.MassDeleteTablesAsync(tableIds);
    }
}
