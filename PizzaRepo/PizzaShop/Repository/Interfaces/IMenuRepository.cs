
using Entity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<Menucategory>> GetCategoriesAsync();
        Task<List<Menuitem>> GetItemsByCategoryAsync(int categoryId);
        Task AddCategoryAsync(Menucategory category);
        Task AddItemAsync(Menuitem item);
        Task DeleteItemAsync(int id);
        Task DeleteItemsAsync(List<int> itemIds);
    }

}