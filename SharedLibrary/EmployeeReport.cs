using System;

namespace SharedLibrary
{
    public class EmployeeReport
    {
        public int EmpId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public string? Username { get; set; }
        public bool PassNotChanged { get; set; }
        public string? PhotoName { get; set; }
        public string? Manager { get; set; }

    }
}
