public class CustomerFilterModel
    {
        public string SearchTerm { get; set; }
        public string TimePeriod { get; set; } = "All time";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortField { get; set; }
        public bool SortAscending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }