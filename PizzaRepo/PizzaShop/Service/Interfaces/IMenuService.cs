using Entity.Models;
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
        Task<int> AddItemAsync(MenuItemViewModel model);
        Task<Menuitem> GetItemAsync(int id);
        Task<List<Itemandmodifiergroup>> GetItemModifierGroupsAsync(int itemId);
        Task UpdateItemAsync(MenuItemViewModel model);
        Task UpdateItemModifierGroupsAsync(int itemId, List<ModifierGroupSelection> modifierGroups);
        Task DeleteItemAsync(int id);
        Task DeleteCategoryAsync(int id);
        Task DeleteItemsAsync(List<int> itemIds);
        Task<MenuCategoryViewModel> GetCategoryAsync(int id);
        Task UpdateCategoryAsync(MenuCategoryViewModel model);
        Task AddModifierGroupsToItemAsync(int itemId, List<ModifierGroupSelection> modifierGroups);
        Task<(List<MenuItemViewModel> items, int totalItems)> GetItemsByCategoryAsync(int categoryId, int page, int pageSize, string searchTerm);
    }   
}
