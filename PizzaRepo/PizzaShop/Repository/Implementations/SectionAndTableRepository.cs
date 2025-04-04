using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations;

public class SectionAndTableRepository : ISectionAndTableRepository
{
    private readonly ApplicationDbContext _context;
    public SectionAndTableRepository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }
    public async Task<List<Section>> GetSectionsAsync()
    {
        return await _context.Sections
                .Where(c => !c.Isdeleted)
                .ToListAsync();
    }
    public async Task AddSectionAsync(Section section)
    {
        await _context.Sections.AddAsync(section);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteSectionAsync(int id)
    {
        Section section = await _context.Sections.FirstOrDefaultAsync(s => s.Sectionid == id);
        section.Isdeleted = true;
        await _context.SaveChangesAsync();
    }
    public async Task<Section> GetSectionByIdAsync(int id)
    {
        return await _context.Sections.FirstOrDefaultAsync(s => s.Sectionid == id);
    }
    public async Task<bool> UpdateSectionAsync(Section section)
    {
        Section? existingsection = await _context.Sections.FirstOrDefaultAsync(s => s.Sectionid == section.Sectionid);
        if (existingsection == null)
        {
            return false;
        }
        existingsection.Description = section.Description;
        existingsection.Sectionname = section.Sectionname;
        existingsection.Modifieddate = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<(List<DinetableViewModel> tables, int totalItems)> GetTablesBySectionAsync(int sectionId, int page, int pageSize, string searchTerm)
    {
        var query = _context.Dinetables
            .Where(t => t.Sectionid == sectionId && !t.Isdeleted);

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(t => t.Tablename.Contains(searchTerm));
        }

        int totalItems = await query.CountAsync();

        List<DinetableViewModel> tables = await query
            .OrderBy(t => t.Tablename)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new DinetableViewModel
            {
                Tableid = t.Tableid,
                Tablename = t.Tablename,
                Capacity = t.Capacity,
                Status = t.Status,
                Sectionid = t.Sectionid
            })
            .ToListAsync();

        return (tables, totalItems);
    }
    public async Task AddTableAsync(Dinetable table)
    {
        await _context.AddAsync(table);
        await _context.SaveChangesAsync();
    }
    public async Task<Dinetable> GetTableByIdAsync(int id)
    {
        return await _context.Dinetables.FirstOrDefaultAsync(t => t.Tableid == id && !t.Isdeleted);
    }

    public async Task<bool> UpdateTableAsync(Dinetable table)
    {
        Dinetable? existingTable = await _context.Dinetables.FirstOrDefaultAsync(t => t.Tableid == table.Tableid);
        if (existingTable == null)
        {
            return false;
        }

        existingTable.Tablename = table.Tablename;
        existingTable.Capacity = table.Capacity;
        existingTable.Status = table.Status;
        existingTable.Sectionid = table.Sectionid;
        existingTable.Modifieddate = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task DeleteTableAsync(int tableId)
    {
        Dinetable table = await _context.Dinetables.FirstOrDefaultAsync(t => t.Tableid == tableId);
        if (table != null)
        {
            table.Isdeleted = true;
            await _context.SaveChangesAsync();
        }
    }
    public async Task MassDeleteTablesAsync(List<int> ids)
    {
        var tables = await _context.Dinetables
            .Where(t => ids.Contains(t.Tableid))
            .ToListAsync();

        foreach (var table in tables)
        {
            table.Isdeleted = true;
        }

        await _context.SaveChangesAsync();
    }
}
