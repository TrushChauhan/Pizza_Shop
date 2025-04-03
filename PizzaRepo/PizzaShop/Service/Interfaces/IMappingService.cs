using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IMappingService
{
    public MenuCategoryViewModel MapToViewModel(Menucategory category);
    public MenuItemViewModel MapToViewItemModel(Menuitem item);
    public ModifierGroupViewModel MapToViewModifier(Modifiergroup group);
    public ModifierViewModel MapToViewModifier(Modifier modifier);
    public Task<SectionViewModel> MapToViewSectionModel(Section section);

}