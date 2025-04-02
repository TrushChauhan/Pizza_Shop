using Entity.ViewModel;

namespace Service.Interfaces;

public interface ITaxService
{
    Task<List<TaxViewModel>> GetTaxesTableAsync(string search);
    Task DeleteTaxAsync(int id);
    Task AddTaxAsync(TaxViewModel model);
}
