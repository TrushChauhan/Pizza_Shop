
using Entity.Models;
using Entity.ViewModel;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class ModifierService : IModifierService
    {
        private readonly IModifierRepository _modifierRepository;
        private readonly MappingService _mappingService;

        public ModifierService(IModifierRepository modifierRepository, MappingService mappingService)
        {
            _modifierRepository = modifierRepository;
            _mappingService = mappingService;
        }

        public async Task<List<ModifierGroupViewModel>> GetModifierGroupsAsync()
        {
            var groups = await _modifierRepository.GetModifierGroupsAsync();
            return groups.Select(g => _mappingService.ModifierToViewModel(g)).ToList();
        }

        public async Task<List<ModifierViewModel>> GetModifiersByGroupAsync(int modifierGroupId)
        {
            var modifiers = await _modifierRepository.GetModifiersByGroupAsync(modifierGroupId);
            return modifiers.Select(m => _mappingService.MapToViewModifier(m, modifierGroupId)).ToList();
        }

        public async Task AddModifierAsync(ModifierViewModel model)
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
            await _modifierRepository.AddModifierAsync(modifier, model.ModifierGroupId);
        }

        public async Task<int> AddModifierGroupAsync(ModifierGroupViewModel model)
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
            return await _modifierRepository.AddModifierGroupAsync(modifierGroup);
        }

        public async Task<ModifierListResponse> GetAllModifiersAsync(int page = 1, int pageSize = 10, string search = "")
        {
            var modifiers = await _modifierRepository.GetAllModifiersAsync(page, pageSize, search);
            var totalItems = await _modifierRepository.GetAllModifiersCountAsync(search);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return new ModifierListResponse
            {
                Modifiers = modifiers.Select(m => new ModifierItem
                {
                    ModifierId = m.Modifierid,
                    ModifierName = m.Modifiername,
                    Unit = m.Unit,
                    Rate = m.Rate,
                    Quantity = m.Quantity
                }).ToList(),
                TotalPages = totalPages
            };
        }
        public async Task DeleteModifierGroupAsync(int id){
            await _modifierRepository.DeleteModifierGroupAsync(id);
        }
        public async Task AddModifiersToGroupAsync(int modifierGroupId, List<int> modifierIds)
        {
            await _modifierRepository.AddModifiersToGroupAsync(modifierGroupId, modifierIds);
        }

        public async Task RemoveModifierFromGroupAsync(int modifierGroupId, int modifierId)
        {
            await _modifierRepository.RemoveModifierFromGroupAsync(modifierGroupId, modifierId);
        }

        public async Task DeleteModifiersFromGroupAsync(int modifierGroupId, List<int> modifierIds)
        {
            await _modifierRepository.DeleteModifiersFromGroupAsync(modifierGroupId, modifierIds);
        }
    }
}