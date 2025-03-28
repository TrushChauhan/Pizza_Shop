
using Entity.Models;
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
            var items = await _context.Menuitems.Where(c=> c.Categoryid == id).ToListAsync();
            foreach (var item in items){
                item.Isdeleted=true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddItemAsync(Menuitem item)
        {
            await _context.Menuitems.AddAsync(item);
            await _context.SaveChangesAsync();
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

            foreach (var item in items)
            {
                item.Isdeleted = true;
                item.Modifieddate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}