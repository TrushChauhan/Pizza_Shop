using Entity.Models;
using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interfaces;

namespace Service.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly MappingService _mappingService;
        public MenuService(IMenuRepository menuRepository, MappingService mappingService)
        {
            _menuRepository = menuRepository;
            _mappingService = mappingService;

        }

        public async Task<List<MenuCategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await _menuRepository.GetCategoriesAsync();
            return categories.Select(c => _mappingService.MapToViewModel(c)).ToList();
        }

        public async Task<List<MenuItemViewModel>> GetItemsByCategoryAsync(int categoryId)
        {
            var items = await _menuRepository.GetItemsByCategoryAsync(categoryId);
            return items.Select(i => _mappingService.MapToViewItemModel(i)).ToList();
        }
        public async Task<(List<MenuItemViewModel> items, int totalItems)> GetItemsByCategoryAsync(int categoryId, int page, int pageSize, string searchTerm)
        {
            return await _menuRepository.GetItemsByCategoryAsync(categoryId,  page, pageSize,searchTerm);
        }
        public async Task AddCategoryAsync(MenuCategoryViewModel model)
        {
            var category = new Menucategory
            {
                Categoryname = model.CategoryName,
                Description = model.Description,
                Createddate = DateTime.Now,
                Isdeleted = false
            };
            await _menuRepository.AddCategoryAsync(category);
        }

        public async Task<int> AddItemAsync(MenuItemViewModel model)
        {
            int itemid = await _menuRepository.AddItemAsync(model);
            return itemid;
        }

        public async Task AddModifierGroupsToItemAsync(int itemId, List<ModifierGroupSelection> modifierGroups)
        {
            await _menuRepository.AddModifierGroupsToItemAsync(itemId, modifierGroups);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _menuRepository.DeleteCategoryAsync(id);
        }
        public async Task DeleteItemAsync(int id)
        {
            await _menuRepository.DeleteItemAsync(id);
        }

        public async Task DeleteItemsAsync(List<int> itemIds)
        {
            await _menuRepository.DeleteItemsAsync(itemIds);
        }

        public async Task<MenuCategoryViewModel> GetCategoryAsync(int id)
        {
            var category = await _menuRepository.GetCategoryAsync(id);
            if (category == null) throw new KeyNotFoundException("Category not found");

            return new MenuCategoryViewModel
            {
                CategoryId = category.Categoryid,
                CategoryName = category.Categoryname,
                Description = category.Description
            };
        }

        public async Task UpdateCategoryAsync(MenuCategoryViewModel model)
        {
            var category = await _menuRepository.GetCategoryAsync(model.CategoryId);
            if (category == null) throw new KeyNotFoundException("Category not found");

            category.Categoryname = model.CategoryName;
            category.Description = model.Description;
            category.Modifieddate = DateTime.Now;

            await _menuRepository.UpdateCategoryAsync(category);
        }
        public async Task<Menuitem> GetItemAsync(int id)
        {
            return await _menuRepository.GetItemAsync(id);
        }

        public async Task<List<Itemandmodifiergroup>> GetItemModifierGroupsAsync(int itemId)
        {
            return await _menuRepository.GetItemModifierGroupsAsync(itemId);
        }

        public async Task UpdateItemAsync(MenuItemViewModel model)
        {
            await _menuRepository.UpdateItemAsync(model);
        }

        public async Task UpdateItemModifierGroupsAsync(int itemId, List<ModifierGroupSelection> modifierGroups)
        {
            await _menuRepository.UpdateItemModifierGroupsAsync(itemId, modifierGroups);
        }
    }
}