using Entity.Models;
using Entity.ViewModel;

namespace Repository.Interfaces
{

    public interface ITaxRepository
    {
        Task<List<TaxViewModel>> GetTaxesTableAsync(string search);
        Task DeleteTaxAsync(int taxid);
        Task AddTaxAsync(Tax model);
        Task<TaxViewModel> GetTaxByIdAsync(int id);
        Task<bool> UpdateTaxAsync(Tax model);
    }
}