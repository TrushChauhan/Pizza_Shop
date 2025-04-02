using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations;

public class SectionAndTableRepository : ISectionAndTableRepository
{
    private readonly ApplicationDbContext _context;
    public SectionAndTableRepository(ApplicationDbContext dbContext){
        _context=dbContext;
    }
    public async Task<List<Section>> GetSectionsAsync(){
        return await _context.Sections
                .Where(c => !c.Isdeleted)
                .ToListAsync();
    }
}
