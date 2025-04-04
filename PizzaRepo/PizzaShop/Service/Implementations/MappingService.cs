namespace Service.Implementations;
using Entity.Models;
using Entity.ViewModel;

public class MappingService
{
    private readonly ApplicationDbContext _context;
    public MappingService(ApplicationDbContext context)
    {
        _context = context;
    }
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
    public MenuItemViewModel MapToViewItemModel(Menuitem item)
    {
        return new MenuItemViewModel
        {
            ItemId = item.Itemid,
            CategoryId = item.Categoryid,
            ItemName = item.Itemname,
            ItemType = item.Itemtype,
            Rate = item.Rate,
            Unit = item.Unit,
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
            CategoryName = item.Category?.Categoryname
        };
    }
    public ModifierGroupViewModel ModifierToViewModel(Modifiergroup group)
    {
        return new ModifierGroupViewModel
        {
            ModifierGroupId = group.Modifiergroupid,
            ModifierGroupName = group.Modifiergroupname,
            Description = group.Description,
            CreatedDate = group.Createddate,
            ModifiedDate = group.Modifieddate,
            IsDeleted = group.Isdeleted
        };
    }

    public ModifierViewModel MapToViewModifier(Modifier modifier, int modifierGroupId)
    {
        var group = _context.Modifiergroups.Find(modifierGroupId);
        return new ModifierViewModel
        {
            ModifierId = modifier.Modifierid,
            ModifierGroupId = modifierGroupId,
            ModifierGroupName = group?.Modifiergroupname ?? "N/A",
            ModifierName = modifier.Modifiername,
            Unit = modifier.Unit,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            Description = modifier.Description,
            CreatedDate = modifier.Createddate,
            ModifiedDate = modifier.Modifieddate,
            IsDeleted = modifier.Isdeleted
        };
    }
    public async Task<SectionViewModel> MapToViewSectionModel(Section section)
    {
        return new SectionViewModel
        {
            Sectionid = section.Sectionid,
            Sectionname = section.Sectionname,
            Description = section.Description,
            Createddate = section.Createddate,
            Modifieddate = section.Modifieddate,
            Isdeleted = section.Isdeleted,
        };
    }
    public async Task<DinetableViewModel> MapToViewTableModel(Dinetable table)
    {
        return new DinetableViewModel
        {
            Tableid = table.Tableid,
            Tablename = table.Tablename,
            Capacity = table.Capacity,
            Status = table.Status,
            Sectionid = table.Sectionid
        };
    }
}