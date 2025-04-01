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
}
