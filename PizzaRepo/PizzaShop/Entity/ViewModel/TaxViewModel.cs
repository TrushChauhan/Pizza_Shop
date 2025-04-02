namespace Entity.ViewModel;

public class TaxViewModel
{
    
    public int Taxid { get; set; }
    public string Taxname { get; set; } = null!;
    public string Taxtype { get; set; } = null!;
    public bool Isenabled { get; set; }
    public double Taxamount { get; set; }
    public DateTime Createddate { get; set; }
    public DateTime? Modifieddate { get; set; }
    public int? Createdby { get; set; }
    public int? Modifiedby { get; set; }
    public bool Isdeleted { get; set; }

}
