using Entity.ViewModel;

namespace Service.Interfaces;

public interface IModifierService
    {
        Task<List<ModifierGroupViewModel>> GetModifierGroupsAsync();
        Task<List<ModifierViewModel>> GetModifiersByGroupAsync(int modifierGroupId);
        Task AddModifierAsync(ModifierViewModel model);
        Task<int> AddModifierGroupAsync(ModifierGroupViewModel model);
        Task<ModifierListResponse> GetAllModifiersAsync(int page = 1, int pageSize = 10, string search = "");
        Task AddModifiersToGroupAsync(int modifierGroupId, List<int> modifierIds);
        Task DeleteModifierGroupAsync(int id);
        Task RemoveModifierFromGroupAsync(int modifierGroupId, int modifierId);
        Task DeleteModifiersFromGroupAsync(int modifierGroupId, List<int> modifierIds);
        Task<ModifierGroupViewModel> GetModifierGroupAsync(int id);
        Task UpdateModifierGroupAsync(ModifierGroupViewModel model);
        Task<ModifierViewModel> GetModifierAsync(int id);
        Task UpdateModifierAsync(ModifierViewModel model);
        Task<(List<ModifierViewModel> modifiers, int totalModifiers)> GetModifiersByGroupAsync(int categoryId, int page, int pageSize, string searchTerm);
    }
