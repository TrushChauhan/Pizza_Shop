namespace Service.Implementations;
using Entity.Models;
using Entity.ViewModel;
public class MappingService
{
    // Map Menucategory to MenuCategoryViewModel
    public MenuCategoryViewModel MapToViewModel(Menucategory category)
    {
        return new MenuCategoryViewModel
        {
            CategoryId = category.Categoryid,
            CategoryName = category.Categoryname,
            Description = category.Description,
            CreatedDate = category.Createddate,
            ModifiedDate = category.Modifieddate,
            IsDeleted = category.Isdeleted,
        };
    }
    // Map Menuitem to MenuItemViewModel
    public MenuItemViewModel MapToViewModel(Menuitem item)
    {
        return new MenuItemViewModel
        {
            ItemId = item.Itemid,
            CategoryId = item.Categoryid,
            ItemName = item.Itemname,
            ItemType = item.Itemtype,
            Rate = item.Rate,
            Quantity = item.Quantity,
            Available = item.Available,
            Shortcode = item.Shortcode,
            ItemImage = item.Itemimage,
            Description = item.Description,
            CreatedDate = item.Createddate,
            ModifiedDate = item.Modifieddate,
            IsDeleted = item.Isdeleted,
            IsFavourite = item.Isfavourite,
            IsDefaultTax = item.Isdefaulttax,
            TaxId = item.Taxid,
            CategoryName = item.Category?.Categoryname // Example
        };
    }
}