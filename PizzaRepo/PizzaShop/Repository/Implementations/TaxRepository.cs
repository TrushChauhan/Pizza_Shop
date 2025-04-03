using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;

namespace Repository.Implementations;

public class TaxRepository : ITaxRepository
{
    private readonly ApplicationDbContext _context;
    public TaxRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<TaxViewModel>> GetTaxesTableAsync(string search)
{
    var query = _context.Taxes
        .Where(t => !t.Isdeleted);

    if (!string.IsNullOrEmpty(search))
    {
        search = await RemoveWhitespace(search);
        search = search.ToLower().Trim();
        query = query.Where(t => 
            t.Taxname.ToLower().Contains(search));
    }

    return await query
        .OrderBy(t => t.Taxname)
        .Select(t => new TaxViewModel
        {
            Taxid = t.Taxid,
            Taxname = t.Taxname,
            Taxamount = t.Taxamount,
            Taxtype = t.Taxtype,
            Isenabled = t.Isenabled
        })
        .ToListAsync();
}

    public async Task<string> RemoveWhitespace(string input)
    {
        return new string(input
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }
    public async Task DeleteTaxAsync(int id)
    {
        var tax = await _context.Taxes.FirstOrDefaultAsync(t => t.Taxid == id);
        tax.Isdeleted = true;
        await _context.SaveChangesAsync();
    }
    public async Task AddTaxAsync(Tax model)
    {
        await _context.Taxes.AddAsync(model);
        await _context.SaveChangesAsync();
    }
    public async Task<TaxViewModel> GetTaxByIdAsync(int id)
    {
        return await _context.Taxes
            .Where(t => t.Taxid == id)
            .Select(t => new TaxViewModel
            {
                Taxid = t.Taxid,
                Taxname = t.Taxname,
                Taxamount = t.Taxamount,
                Taxtype = t.Taxtype,
                Isenabled = t.Isenabled
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateTaxAsync(Tax model)
    {
        var tax = await _context.Taxes.FirstOrDefaultAsync(t => t.Taxid == model.Taxid);
        if (tax == null)
        {
            return false;
        }

        tax.Taxname = model.Taxname;
        tax.Taxamount = model.Taxamount;
        tax.Taxtype = model.Taxtype;
        tax.Isenabled = model.Isenabled;
        tax.Modifieddate = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }
}
