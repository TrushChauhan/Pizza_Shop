using System.ComponentModel.DataAnnotations;

namespace Entity.ViewModel;

public class ModifierViewModel
{
    public int ModifierId { get; set; }
    [Required(ErrorMessage = "Modifier name is required")]
    [StringLength(50, ErrorMessage = "Modifier name cannot exceed 50 characters")]
    public string ModifierName { get; set; }
    [Required(ErrorMessage = "Unit is required")]
    [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
    public string Unit { get; set; }
    [Required(ErrorMessage = "Rate is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Rate must be a positive number")]
    public int Rate { get; set; }
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a positive number")]
    public int Quantity { get; set; }
    [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; }
} 
