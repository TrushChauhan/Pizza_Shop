using Entity.ViewModel;

namespace Service.Interfaces;

public interface ITaxService
{
    Task<List<TaxViewModel>> GetTaxesTableAsync(string search);
    Task DeleteTaxAsync(int id);
    Task AddTaxAsync(TaxViewModel model);
    Task<TaxViewModel> GetTaxByIdAsync(int id);
    Task<bool> UpdateTaxAsync(TaxViewModel model);
}
