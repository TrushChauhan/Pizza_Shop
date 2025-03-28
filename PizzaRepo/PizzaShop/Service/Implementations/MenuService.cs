// MenuService.cs
using Entity.Models;
using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return items.Select(i => _mappingService.MapToViewModel(i)).ToList();
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

        public async Task AddItemAsync(MenuItemViewModel model)
        {
            var item = new Menuitem
            {
                Categoryid = model.CategoryId,
                Itemname = model.ItemName,
                Itemtype = model.ItemType,
                Rate = model.Rate,
                Quantity = model.Quantity,
                Available = model.Available,
                Createddate = DateTime.Now,
                Isdeleted = false
            };
            await _menuRepository.AddItemAsync(item);
        }
        public async Task DeleteCategoryAsync(int id){
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
    }
}