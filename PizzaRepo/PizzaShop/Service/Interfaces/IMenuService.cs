using Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuCategoryViewModel>> GetCategoriesAsync();
        Task<List<MenuItemViewModel>> GetItemsByCategoryAsync(int categoryId);
        Task AddCategoryAsync(MenuCategoryViewModel model);
        Task AddItemAsync(MenuItemViewModel model);
        Task DeleteItemAsync(int id);
        Task DeleteCategoryAsync(int id);
        Task DeleteItemsAsync(List<int> itemIds);
        Task<MenuCategoryViewModel> GetCategoryAsync(int id);
        Task UpdateCategoryAsync(MenuCategoryViewModel model);
        
    }
}
