namespace Entity.ViewModel;

public class OrderViewModel
{
    public int OrderId { get; set; }
    public DateTime Date { get; set; }
    public string CustomerName { get; set; }
    public string Status { get; set; }
    public string PaymentMode { get; set; }
    public int Rating { get; set; }
    public double TotalAmount { get; set; }
}
