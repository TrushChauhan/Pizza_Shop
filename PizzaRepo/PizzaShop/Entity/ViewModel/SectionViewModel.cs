namespace Entity.ViewModel;

public class SectionViewModel
{
    public int Sectionid { get; set; }
    public string Sectionname { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime Createddate { get; set; }
    public DateTime? Modifieddate { get; set; }
    public int? Createdby { get; set; }
    public int? Modifiedby { get; set; }
    public bool Isdeleted { get; set; }
}
