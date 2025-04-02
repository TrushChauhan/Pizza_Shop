using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations;

public class TaxRepository : ITaxRepository
{
    private readonly ApplicationDbContext _context;
    public TaxRepository(ApplicationDbContext context){
        _context=context;
    }
    public async Task<List<TaxViewModel>> GetTaxesTableAsync(string search){
        var taxes = _context.Taxes
                .Where(ud => !ud.Isdeleted);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                search = await RemoveWhitespace(search);
                taxes = taxes.Where(t =>
                    t.Taxname.ToLower().Contains(search));
            }
            
            var taxeslist = await taxes.Select(t=> new TaxViewModel{
                Taxid=t.Taxid,
                Taxname=t.Taxname,
                Taxamount=t.Taxamount,
                Taxtype=t.Taxtype,
                Isenabled=t.Isenabled
            }).ToListAsync();
        return taxeslist;
    }

    public async Task<string> RemoveWhitespace(string input)
        {
            return new string(input
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }
    public async Task DeleteTaxAsync(int id){
       var tax= await _context.Taxes.FirstOrDefaultAsync(t=> t.Taxid==id);
       tax.Isdeleted=true;
       _context.SaveChangesAsync();
    }
   public async Task AddTaxAsync(Tax model){
       await _context.Taxes.AddAsync(model);
       await _context.SaveChangesAsync();
   }
}
