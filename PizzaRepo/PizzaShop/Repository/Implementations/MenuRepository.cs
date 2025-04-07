
using Entity.Models;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Implementations
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Menucategory>> GetCategoriesAsync()
        {
            return await _context.Menucategories
                .Where(c => !c.Isdeleted)
                .ToListAsync();
        }

        public async Task<List<Menuitem>> GetItemsByCategoryAsync(int categoryId)
        {
            return await _context.Menuitems
                .Where(i => i.Categoryid == categoryId && !i.Isdeleted)
                .ToListAsync();
        }

        public async Task AddCategoryAsync(Menucategory category)
        {
            await _context.Menucategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Menucategories.FindAsync(id);
            if (category != null)
            {
                category.Isdeleted = true;
                await _context.SaveChangesAsync();
            }
            var items = await _context.Menuitems.Where(c => c.Categoryid == id).ToListAsync();
            foreach (var item in items)
            {
                item.Isdeleted = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<(List<MenuItemViewModel> items, int totalItems)> GetItemsByCategoryAsync(int categoryId, int page, int pageSize, string searchTerm)
        {
            
            var query = _context.Menuitems
                .Where(i => i.Categoryid == categoryId && !i.Isdeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(i => i.Itemname.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(i => i.Itemname)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new MenuItemViewModel
                {
                    ItemId = i.Itemid,
                    ItemName = i.Itemname,
                    ItemType = i.Itemtype,
                    Rate = i.Rate,
                    Quantity = i.Quantity,
                    Available = i.Available,
                    ItemImage = i.Itemimage
                })
                .ToListAsync();

            return (items, totalItems);
        }
        public async Task<int> AddItemAsync(MenuItemViewModel item)
        {
            var newItem = new Menuitem
            {
                Itemid = await _context.Menuitems.CountAsync() + 1,
                Categoryid = item.CategoryId,
                Itemname = item.ItemName,
                Itemtype = item.ItemType,
                Rate = item.Rate,
                Unit = item.Unit,
                Quantity = item.Quantity,
                Available = item.Available,
                Shortcode = item.Shortcode,
                Itemimage = item.ItemImage,
                Description = item.Description,
                Createddate = DateTime.Now,
                Modifieddate = DateTime.Now,
                Isdeleted = item.IsDeleted,
                Isfavourite = item.IsFavourite,
                Isdefaulttax = item.IsDefaultTax,
                Taxpercentage = item.TaxPercentage
            };
            await _context.Menuitems.AddAsync(newItem);
            await _context.SaveChangesAsync();
            return newItem.Itemid;
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Menuitems.FindAsync(id);
            if (item != null)
            {
                item.Isdeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemsAsync(List<int> itemIds)
        {
            var items = await _context.Menuitems
                .Where(i => itemIds.Contains(i.Itemid))
                .ToListAsync();
            var mappeditems = await _context.Itemandmodifiergroups.Where(i => itemIds.Contains(i.Itemid.Value)).ToListAsync();
            foreach (var item in items)
            {
                item.Isdeleted = true;
                item.Modifieddate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            foreach (var item in mappeditems)
            {
                item.Isdeleted = true;
                item.Modifieddate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

        }
        public async Task<Menucategory> GetCategoryAsync(int id)
        {
            return await _context.Menucategories
                .FirstOrDefaultAsync(c => c.Categoryid == id && !c.Isdeleted);
        }

        public async Task UpdateCategoryAsync(Menucategory category)
        {
            var existing = await _context.Menucategories.FindAsync(category.Categoryid);
            if (existing != null)
            {
                existing.Categoryname = category.Categoryname;
                existing.Description = category.Description;
                existing.Modifieddate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddModifierGroupsToItemAsync(int itemId, List<ModifierGroupSelection> modifierGroups)
        {
            foreach (var group in modifierGroups)
            {
                var mapping = new Itemandmodifiergroup
                {
                    Itemandmodifiergroupid = await _context.Itemandmodifiergroups.CountAsync() + 1,
                    Itemid = itemId,
                    Modifiergroupid = group.ModifierGroupId,
                    Minselect = group.MinSelect,
                    Maxselect = group.MaxSelect,
                    Createddate = DateTime.Now,
                    Modifieddate = DateTime.Now,
                    Isdeleted = false
                };
                await _context.Itemandmodifiergroups.AddAsync(mapping);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Menuitem> GetItemAsync(int id)
        {
            return await _context.Menuitems
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Itemid == id && !i.Isdeleted);
        }

        public async Task<List<Itemandmodifiergroup>> GetItemModifierGroupsAsync(int itemId)
        {
            return await _context.Itemandmodifiergroups
                .Include(img => img.Modifiergroup)
                .Where(img => img.Itemid == itemId && !img.Isdeleted)
                .ToListAsync();
        }

        public async Task UpdateItemAsync(MenuItemViewModel model)
        {
            var item = await _context.Menuitems.FirstOrDefaultAsync(i => i.Itemid == model.ItemId && !i.Isdeleted);
            if (item == null) throw new KeyNotFoundException("Item not found");

            item.Categoryid = model.CategoryId;
            item.Itemname = model.ItemName;
            item.Itemtype = model.ItemType;
            item.Rate = model.Rate;
            item.Quantity = model.Quantity;
            item.Unit = model.Unit;
            item.Available = model.Available;
            item.Shortcode = model.Shortcode;
            item.Description = model.Description;
            item.Isdefaulttax = model.IsDefaultTax;
            item.Taxpercentage = model.TaxPercentage;
            item.Modifieddate = DateTime.Now;

            if (!string.IsNullOrEmpty(model.ItemImage))
            {
                item.Itemimage = model.ItemImage;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemModifierGroupsAsync(int itemId, List<ModifierGroupSelection> modifierGroups)
        {
            var existingMappings = await _context.Itemandmodifiergroups
                .Where(img => img.Itemid == itemId && !img.Isdeleted)
                .ToListAsync();

            foreach (var newGroup in modifierGroups)
            {
                var existingMapping = existingMappings
                    .FirstOrDefault(em => em.Modifiergroupid == newGroup.ModifierGroupId);

                if (existingMapping != null)
                {
                    // Update existing mapping if min/max values changed
                    if (existingMapping.Minselect != newGroup.MinSelect ||
                        existingMapping.Maxselect != newGroup.MaxSelect)
                    {
                        existingMapping.Minselect = newGroup.MinSelect;
                        existingMapping.Maxselect = newGroup.MaxSelect;
                        existingMapping.Modifieddate = DateTime.Now;
                        await _context.SaveChangesAsync();
                    }
                    existingMappings.Remove(existingMapping);
                }
                else
                {
                    var mapping = new Itemandmodifiergroup
                    {
                        Itemandmodifiergroupid = await _context.Itemandmodifiergroups.CountAsync() + 1,
                        Itemid = itemId,
                        Modifiergroupid = newGroup.ModifierGroupId,
                        Minselect = newGroup.MinSelect,
                        Maxselect = newGroup.MaxSelect,
                        Createddate = DateTime.Now,
                        Modifieddate = DateTime.Now,
                        Isdeleted = false
                    };
                    await _context.Itemandmodifiergroups.AddAsync(mapping);
                    await _context.SaveChangesAsync();
                }
            }
            foreach (var oldMapping in existingMappings)
            {
                oldMapping.Isdeleted = true;
                oldMapping.Modifieddate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}