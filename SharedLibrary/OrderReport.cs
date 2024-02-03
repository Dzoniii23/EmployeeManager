namespace SharedLibrary
{
    public class OrderReport
    {
        public int EmpId { get; set; }
        public string? EmpName { get; set; }
        public int OrderId { get; set; }
        public int CustId { get; set; }
        public string? CompanyName { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime RequiredDate { get; set; }
        public int State { get; set; }
    }
}
