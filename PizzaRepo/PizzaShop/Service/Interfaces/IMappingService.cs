using Entity.Models;
using Entity.ViewModel;

namespace Service.Interfaces;

public interface IMappingService
{
    public MenuCategoryViewModel MapToViewModel(Menucategory category);
    public MenuItemViewModel MapToViewModel(Menuitem item);
}
