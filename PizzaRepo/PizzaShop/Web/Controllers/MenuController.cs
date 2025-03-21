using Microsoft.AspNetCore.Mvc;
using Entity.Models;
using Entity.ViewModel;
using System.Linq;
using Service.Interfaces;
using Service.Implementations;
public class MenuController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly MappingService _mappingService;
    public MenuController(ApplicationDbContext context, MappingService mappingService)
    {
        _context = context;
        _mappingService = mappingService;
    }
    // GET: Menu/Index
    public IActionResult Index()
    {
        var categories = _context.Menucategories
            .Where(c => !c.Isdeleted)
            .ToList();
        var categoryViewModels = categories
            .Select(c => _mappingService.MapToViewModel(c))
            .ToList();
        return View(categoryViewModels);
    }
    // GET: Menu/GetItems?categoryId=1
    public IActionResult GetItems(int categoryId)
    {
        var items = _context.Menuitems
            .Where(i => i.Categoryid == categoryId && !i.Isdeleted)
            .ToList();
        var itemViewModels = items
            .Select(i => _mappingService.MapToViewModel(i))
            .ToList();
        return Json(itemViewModels);
    }
    // POST: Menu/AddCategory
    [HttpPost]
    public IActionResult AddCategory([FromBody] MenuCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var category = new Menucategory
            {
                Categoryname = model.CategoryName,
                Description = model.Description,
                Createddate = DateTime.Now,
                Isdeleted = false
            };
            _context.Menucategories.Add(category);
            _context.SaveChanges();
            return Ok();
        }
        return BadRequest(ModelState);
    }
    // POST: Menu/AddItem
    [HttpPost]
    public IActionResult AddItem([FromBody] MenuItemViewModel model)
    {
        if (ModelState.IsValid)
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
            _context.Menuitems.Add(item);
            _context.SaveChanges();
            return Ok();
        }
        return BadRequest(ModelState);
    }
    // POST: Menu/DeleteItem/1
    [HttpPost]
    public IActionResult DeleteItem(int id)
    {
        var item = _context.Menuitems.Find(id);
        if (item == null)
        {
            return NotFound();
        }
        item.Isdeleted = true;
        _context.SaveChanges();
        return Ok();
    }
    public IActionResult GetModifierGroups()
    {
        var modifierGroups = _context.Modifiergroups
        .Where(mg => !mg.Isdeleted)
        .Select(mg => new ModifierGroupViewModel
        {
            ModifierGroupId = mg.Modifiergroupid,
            ModifierGroupName = mg.Modifiergroupname,
            Description = mg.Description,
            MinSelect = mg.Minselect,
            MaxSelect = mg.Maxselect,
            CreatedDate = mg.Createddate,
            ModifiedDate = mg.Modifieddate,
            IsDeleted = mg.Isdeleted,
            Modifiers = mg.Modifiergroupandmodifiers
        .Where(mgm => !mgm.Isdeleted)
        .Select(mgm => new ModifierViewModel
        {
            ModifierId = mgm.Modifier.Modifierid,
            ModifierName = mgm.Modifier.Modifiername,
            Unit = mgm.Modifier.Unit,
            Rate = mgm.Modifier.Rate,
            Quantity = mgm.Modifier.Quantity,
            Description = mgm.Modifier.Description,
            CreatedDate = mgm.Modifier.Createddate,
            ModifiedDate = mgm.Modifier.Modifieddate,
            IsDeleted = mgm.Modifier.Isdeleted
        })
        .ToList()
        })
        .ToList();
        return Json(modifierGroups);
    }
    [HttpPost]
public IActionResult AddModifierGroup([FromBody] ModifierGroupViewModel model)
{
    if (ModelState.IsValid)
    {
        var modifierGroup = new Modifiergroup
        {
            Modifiergroupname = model.ModifierGroupName,
            Description = model.Description,
            Minselect = model.MinSelect,
            Maxselect = model.MaxSelect,
            Createddate = DateTime.Now,
            Isdeleted = false
        };
        _context.Modifiergroups.Add(modifierGroup);
        _context.SaveChanges();
        return Ok();
    }
    return BadRequest(ModelState);
} 
[HttpPost]
public IActionResult AddModifier([FromBody] ModifierViewModel model)
{
    if (ModelState.IsValid)
    {
        var modifier = new Modifier
        {
            Modifiername = model.ModifierName,
            Unit = model.Unit,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Description = model.Description,
            Createddate = DateTime.Now,
            Isdeleted = false
        };
        _context.Modifiers.Add(modifier);
        _context.SaveChanges();
        return Ok();
    }
    return BadRequest(ModelState);
} 
}