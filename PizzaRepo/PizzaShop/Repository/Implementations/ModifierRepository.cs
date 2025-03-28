// ModifierRepository.cs
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Implementations
{
    public class ModifierRepository : IModifierRepository
    {
        private readonly ApplicationDbContext _context;

        public ModifierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Modifiergroup>> GetModifierGroupsAsync()
        {
            return await _context.Modifiergroups
                .Where(g => !g.Isdeleted)
                .ToListAsync();
        }

        public async Task<List<Modifier>> GetModifiersByGroupAsync(int modifierGroupId)
        {
            return await _context.Modifiergroupandmodifiers
                .Where(m => m.Modifiergroupid == modifierGroupId && !m.Isdeleted)
                .Join(_context.Modifiers.Where(m => !m.Isdeleted),
                    mg => mg.Modifierid,
                    m => m.Modifierid,
                    (mg, m) => m)
                .ToListAsync();
        }

        public async Task AddModifierAsync(Modifier modifier, int modifierGroupId)
        {
            modifier.Modifierid= await _context.Modifiers.CountAsync()+1;
            await _context.Modifiers.AddAsync(modifier);
            await _context.SaveChangesAsync();

            var junction = new Modifiergroupandmodifier
            {
                Modifiergroupid = modifierGroupId,
                Modifierid = modifier.Modifierid,
                Createdat = DateTime.Now,
                Isdeleted = false
            };

            await _context.Modifiergroupandmodifiers.AddAsync(junction);
            await _context.SaveChangesAsync();
        }

        public async Task<int> AddModifierGroupAsync(Modifiergroup modifierGroup)
        {
            await _context.Modifiergroups.AddAsync(modifierGroup);
            await _context.SaveChangesAsync();
            return modifierGroup.Modifiergroupid;
        }

        public async Task<List<Modifier>> GetAllModifiersAsync(int page = 1, int pageSize = 10, string search = "")
        {
            var query = _context.Modifiers.Where(m => !m.Isdeleted);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(m => m.Modifiername.ToLower().Contains(search));
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetAllModifiersCountAsync(string search = "")
        {
            var query = _context.Modifiers.Where(m => !m.Isdeleted);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Modifiername.Contains(search));
            }
            return await query.CountAsync();
        }

        public async Task AddModifiersToGroupAsync(int modifierGroupId, List<int> modifierIds)
        {
            foreach (var modifierId in modifierIds)
            {
                await _context.Modifiergroupandmodifiers.AddAsync(new Modifiergroupandmodifier
                {
                    Mandmid=await _context.Modifiergroupandmodifiers.CountAsync() + 1,
                    Modifiergroupid = modifierGroupId,
                    Modifierid = modifierId,
                    Createdat = DateTime.Now,
                    Isdeleted = false
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveModifierFromGroupAsync(int modifierGroupId, int modifierId)
        {
            var junction = await _context.Modifiergroupandmodifiers
                .FirstOrDefaultAsync(m => m.Modifiergroupid == modifierGroupId
                                    && m.Modifierid == modifierId);

            if (junction != null)
            {
                junction.Isdeleted = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteModifierGroupAsync(int id){
            var modifierGroup = await _context.Modifiergroups.FindAsync(id);
            if (modifierGroup != null)
            {
                modifierGroup.Isdeleted = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteModifiersFromGroupAsync(int modifierGroupId, List<int> modifierIds)
        {
            var groupModifiers = await _context.Modifiergroupandmodifiers
                .Where(m => m.Modifiergroupid == modifierGroupId &&
                            modifierIds.Contains(m.Modifierid) &&
                            !m.Isdeleted)
                .ToListAsync();

            foreach (var gm in groupModifiers)
            {
                gm.Isdeleted = true;
                gm.Modifiedat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
        public async Task<Modifiergroup> GetModifierGroupAsync(int id)
        {
            return await _context.Modifiergroups
                .FirstOrDefaultAsync(g => g.Modifiergroupid == id && !g.Isdeleted);
        }

        public async Task UpdateModifierGroupAsync(Modifiergroup group)
        {
            var existing = await _context.Modifiergroups.FindAsync(group.Modifiergroupid);
            if (existing != null)
            {
                existing.Modifiergroupname = group.Modifiergroupname;
                existing.Description = group.Description;
                existing.Modifieddate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
