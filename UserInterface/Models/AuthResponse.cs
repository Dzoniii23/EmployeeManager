using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;

namespace UserInterface.Models
{
    internal class AuthResponse
    {
        public string? Token { get; set; }
        public EmployeeReport? EmployeeReport { get; set; }
    }
}
