namespace Entity.ViewModel
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public string PaymentMode { get; set; }
        public DateTime PaidOn { get; set; }
        public DateTime PlacedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public TimeSpan OrderDuration { get; set; }
        public double TotalAmount { get; set; }
        public double SubTotal { get; set; }
        
        
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int NumberOfPersons { get; set; }
        
        public string TableName { get; set; }
        public string SectionName { get; set; }
        
        public List<OrderItemViewModel> OrderItems { get; set; }
        
        public List<OrderTaxViewModel> Taxes { get; set; }
    }

    public class OrderItemViewModel
    {
        public int SrNo { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderItemModifierViewModel> Modifiers { get; set; }
    }

    public class OrderItemModifierViewModel
    {
        public string ModifierName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
    }

    public class OrderTaxViewModel
    {
        public string TaxName { get; set; }
        public double TaxValue { get; set; }
    }
}