using Microsoft.AspNetCore.Mvc;
using Entity.ViewModel;
using Service.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IModifierService _modifierService;

        public MenuController(IMenuService menuService, IModifierService modifierService)
        {
            _menuService = menuService;
            _modifierService = modifierService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _menuService.GetCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> GetItems(int categoryId)
        {
            var items = await _menuService.GetItemsByCategoryAsync(categoryId);
            return Json(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] MenuCategoryViewModel model)
        {
            await _menuService.AddCategoryAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _menuService.DeleteCategoryAsync(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] MenuItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _menuService.AddItemAsync(model);
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _menuService.DeleteItemAsync(id);
            return Ok();
        }

        public async Task<IActionResult> GetModifierGroups()
        {
            var groups = await _modifierService.GetModifierGroupsAsync();
            return Json(groups);
        }

        public async Task<IActionResult> GetModifiers(int modifierGroupId)
        {
            var modifiers = await _modifierService.GetModifiersByGroupAsync(modifierGroupId);
            return Json(modifiers);
        }

        [HttpPost]
        public async Task<IActionResult> AddModifier([FromBody] ModifierViewModel model)
        {
        
                await _modifierService.AddModifierAsync(model);
                return Ok();
            
        }

        [HttpPost]
        public async Task<IActionResult> AddModifierGroup([FromBody] ModifierGroupViewModel model)
        {
            int modifierid= await _modifierService.AddModifierGroupAsync(model);

            return Json(new { modifierGroupId = modifierid });
            
        }
        public async Task<IActionResult> DeleteModifierGroup(int id)
        {
            await _modifierService.DeleteModifierGroupAsync(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllModifiers(int page = 1, int pageSize = 10, string search = "")
        {
            var result = await _modifierService.GetAllModifiersAsync(page, pageSize, search);
            return Json(new
            {
                modifiers = result.Modifiers,
                totalPages = result.TotalPages
            });
        }

        // Get single category endpoint
        [HttpGet]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _menuService.GetCategoryAsync(id);
            return Json(category);
        }

        // Update category endpoint
        [HttpPost]
        public async Task<IActionResult> UpdateCategory([FromBody] MenuCategoryViewModel model)
        {
            await _menuService.UpdateCategoryAsync(model);
            return Ok();
        }
        // Get single modifier group endpoint
        [HttpGet]
        public async Task<IActionResult> GetModifierGroup(int id)
        {
            var group = await _modifierService.GetModifierGroupAsync(id);
            return Json(group);
        }

        // Update modifier group endpoint
        [HttpPost]
        public async Task<IActionResult> UpdateModifierGroup([FromBody] ModifierGroupViewModel model)
        {
            await _modifierService.UpdateModifierGroupAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddModifiersToGroup([FromBody] AddModifiersToGroupRequest request)
        {
            await _modifierService.AddModifiersToGroupAsync(request.ModifierGroupId, request.ModifierIds);
            return Ok();
        }
        

        [HttpPost]
        public async Task<IActionResult> RemoveModifierFromGroup([FromBody] RemoveModifierRequest request)
        {
            await _modifierService.RemoveModifierFromGroupAsync(request.ModifierGroupId, request.ModifierId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItems([FromBody] List<int> itemIds)
        {
            await _menuService.DeleteItemsAsync(itemIds);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteModifiersFromGroup([FromBody] DeleteModifiersFromGroupRequest request)
        {
            await _modifierService.DeleteModifiersFromGroupAsync(request.ModifierGroupId, request.ModifierIds);
            return Ok();
        }

        public class RemoveModifierRequest
        {
            public int ModifierGroupId { get; set; }
            public int ModifierId { get; set; }
        }

        public class AddModifiersToGroupRequest
        {
            public int ModifierGroupId { get; set; }
            public List<int> ModifierIds { get; set; }
        }

        public class DeleteModifiersFromGroupRequest
        {
            public int ModifierGroupId { get; set; }
            public List<int> ModifierIds { get; set; }
        }
    }
}