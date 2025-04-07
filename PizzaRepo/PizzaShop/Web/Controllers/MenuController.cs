using Microsoft.AspNetCore.Mvc;
using Entity.ViewModel;
using Service.Interfaces;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IModifierService _modifierService;
        private readonly IFileService _fileService;
        private readonly INotyfService _notify;
        public MenuController(IMenuService menuService, IModifierService modifierService, IFileService fileService, INotyfService notify)
        {
            _notify = notify;
            _menuService = menuService;
            _modifierService = modifierService;
            _fileService = fileService;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var categories = await _menuService.GetCategoriesAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] MenuCategoryViewModel model)
        {
            await _menuService.AddCategoryAsync(model);
            _notify.Custom("Category Added Successfully", 5, "Green", "fa-regular fa-check");
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetItems(int categoryId, int page = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var (items, totalItems) = await _menuService.GetItemsByCategoryAsync(categoryId, page, pageSize, searchTerm);

                return Ok(new
                {
                    items = items,
                    totalItems = totalItems
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _menuService.DeleteCategoryAsync(id);
            _notify.Custom("Category Deleted Successfully", 5, "Green", "fa-regular fa-check");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _menuService.DeleteItemAsync(id);
            _notify.Custom("Item Deleted Successfully", 5, "Green", "fa-regular fa-check");
            return Ok();
        }

        public async Task<IActionResult> GetModifierGroups()
        {
            var groups = await _modifierService.GetModifierGroupsAsync();
            return Json(groups);
        }

        [HttpGet]
        public async Task<IActionResult> GetModifiers(int modifierGroupId, int page = 1, int pageSize = 10, string searchTerm = "")
        {
            try
            {
                var (modifiers, totalModifiers) = await _modifierService.GetModifiersByGroupAsync(modifierGroupId, page, pageSize, searchTerm);

                return Ok(new
                {
                    modifiers = modifiers,
                    totalModifiers = totalModifiers
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddModifier([FromBody] ModifierViewModel model)
        {
            await _modifierService.AddModifierAsync(model);
            _notify.Custom("Modifier Added Successfully", 5, "Green", "fa-regular fa-check");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddModifierGroup([FromBody] ModifierGroupViewModel model)
        {
            int modifierid = await _modifierService.AddModifierGroupAsync(model);
            _notify.Custom("Modifier Group Added Successfully", 5, "Green", "fa-regular fa-check");
            return Json(new { modifierGroupId = modifierid });

        }
        public async Task<IActionResult> DeleteModifierGroup(int id)
        {
            await _modifierService.DeleteModifierGroupAsync(id);
            _notify.Custom("Modifier Group Deleted Successfully", 5, "Green", "fa-regular fa-check");
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
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _menuService.GetCategoriesAsync();
            return Ok(categories);
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
            _notify.Custom("Category Updated Successfully", 5, "Green", "fa-solid fa-check");
            return Ok();
        }
        // Get single modifier group endpoint
        [HttpGet]
        public async Task<IActionResult> GetModifierGroup(int id)
        {
            var group = await _modifierService.GetModifierGroupAsync(id);
            return Json(group);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModifierGroup([FromBody] ModifierGroupViewModel model)
        {
            await _modifierService.UpdateModifierGroupAsync(model);
            _notify.Custom("Modifier Group Updated Successfully", 5, "Green", "fa-solid fa-check");
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
        [HttpGet]
        public async Task<IActionResult> GetModifier(int id)
        {
            var modifier = await _modifierService.GetModifierAsync(id);
            return Json(modifier);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateModifier([FromBody] ModifierViewModel model)
        {

            try
            {
                await _modifierService.UpdateModifierAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(
    [FromForm] MenuItemViewModel model,
    IFormFile itemImage)
        {
            try
            {
                if (itemImage != null && itemImage.Length > 0)
                {
                    var imagePath = await _fileService.SaveItemImageAsync(itemImage);
                    model.ItemImage = imagePath;
                }

                var modifierGroups = new List<ModifierGroupSelection>();

                var modifierGroupKeys = Request.Form.Keys
                    .Where(k => k.StartsWith("ModifierGroups["))
                    .Select(k => k.Split('[', ']')[1])
                    .Distinct()
                    .ToList();

                foreach (var index in modifierGroupKeys)
                {
                    var groupIdKey = $"ModifierGroups[{index}].ModifierGroupId";
                    if (!Request.Form.ContainsKey(groupIdKey)) continue;

                    var groupId = Request.Form[groupIdKey].FirstOrDefault();
                    var minSelect = Request.Form[$"ModifierGroups[{index}].MinSelect"].FirstOrDefault();
                    var maxSelect = Request.Form[$"ModifierGroups[{index}].MaxSelect"].FirstOrDefault();

                    if (int.TryParse(groupId, out var gId) &&
                        int.TryParse(minSelect, out var min) &&
                        int.TryParse(maxSelect, out var max))
                    {
                        // Check if this group is already added
                        if (!modifierGroups.Any(mg => mg.ModifierGroupId == gId))
                        {
                            modifierGroups.Add(new ModifierGroupSelection
                            {
                                ModifierGroupId = gId,
                                MinSelect = min,
                                MaxSelect = max
                            });
                        }
                    }
                }
                var itemId = await _menuService.AddItemAsync(model);

                if (modifierGroups.Any())
                {
                    await _menuService.AddModifierGroupsToItemAsync(itemId, modifierGroups);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _menuService.GetItemAsync(id);
            if (item == null) return NotFound();

            return Json(new
            {
                itemId = item.Itemid,
                categoryId = item.Categoryid,
                itemName = item.Itemname,
                itemType = item.Itemtype,
                rate = item.Rate,
                quantity = item.Quantity,
                unit = item.Unit,
                available = item.Available,
                shortcode = item.Shortcode,
                itemImage = item.Itemimage,
                description = item.Description,
                isDefaultTax = item.Isdefaulttax,
                taxPercentage = item.Taxpercentage
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetItemModifierGroups(int itemId)
        {
            var groups = await _menuService.GetItemModifierGroupsAsync(itemId);
            return Json(groups.Select(g => new
            {
                modifierGroupId = g.Modifiergroupid,
                modifierGroupName = g.Modifiergroup?.Modifiergroupname,
                minSelect = g.Minselect,
                maxSelect = g.Maxselect
            }));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateItem(
    [FromForm] MenuItemViewModel model,
    IFormFile itemImage)
        {
            try
            {
                if (itemImage != null && itemImage.Length > 0)
                {
                    var imagePath = await _fileService.SaveItemImageAsync(itemImage);
                    model.ItemImage = imagePath;
                }

                var modifierGroups = new List<ModifierGroupSelection>();

                // Get all modifier group keys
                var modifierGroupKeys = Request.Form.Keys
                    .Where(k => k.StartsWith("ModifierGroups["))
                    .Select(k =>
                    {
                        var parts = k.Split('[', ']');
                        return parts.Length > 1 ? parts[1] : null;
                    })
                    .Where(k => k != null)
                    .Distinct()
                    .ToList();

                foreach (var index in modifierGroupKeys)
                {
                    var groupIdKey = $"ModifierGroups[{index}].ModifierGroupId";
                    if (!Request.Form.ContainsKey(groupIdKey)) continue;

                    var groupId = Request.Form[groupIdKey].FirstOrDefault();
                    var minSelect = Request.Form[$"ModifierGroups[{index}].MinSelect"].FirstOrDefault();
                    var maxSelect = Request.Form[$"ModifierGroups[{index}].MaxSelect"].FirstOrDefault();

                    if (int.TryParse(groupId, out var gId) &&
                        int.TryParse(minSelect, out var min) &&
                        int.TryParse(maxSelect, out var max))
                    {
                        modifierGroups.Add(new ModifierGroupSelection
                        {
                            ModifierGroupId = gId,
                            MinSelect = min,
                            MaxSelect = max
                        });
                    }
                }

                // Update the item
                await _menuService.UpdateItemAsync(model);

                if (modifierGroups.Any() || modifierGroupKeys.Any())
                {
                    await _menuService.UpdateItemModifierGroupsAsync(model.ItemId, modifierGroups);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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