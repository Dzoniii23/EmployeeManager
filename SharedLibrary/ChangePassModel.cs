using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class ChangePassModel
    {
        [Required(ErrorMessage = "Employee ID is required")] //data annotation attributes
        public int EmpId { get; set; } = 0;

        [Required(ErrorMessage = "Old password is required")] //data annotation attributes
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")] //data annotation attributes
        public string NewPassword { get; set; } = string.Empty;
    }
}
