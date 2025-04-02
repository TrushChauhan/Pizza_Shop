using Entity.Models;
using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations;

public class TaxService : ITaxService
{
    private readonly ITaxRepository _taxRepo;

    public TaxService(ITaxRepository taxRepository){
        _taxRepo=taxRepository;
    }
    public async Task<List<TaxViewModel>> GetTaxesTableAsync(string search){
        return await _taxRepo.GetTaxesTableAsync(search);
    }
    public async Task DeleteTaxAsync(int id){
        await _taxRepo.DeleteTaxAsync(id);
    }
    public async Task AddTaxAsync(TaxViewModel model){
        var tax = new Tax
            {
                Taxname = model.Taxname,
                Taxamount = model.Taxamount,
                Taxtype=model.Taxtype,
                Createddate = DateTime.Now,
                Isdeleted = false
            };
        await _taxRepo.AddTaxAsync(tax);
    }
}
