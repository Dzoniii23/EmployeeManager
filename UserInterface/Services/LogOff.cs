using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface.Services
{
    public class LogOff
    {
        public LogOff() { }

        public void PerformLogOff()
        {
            // Clear user session information
            CurrentUser.Token = null;
            CurrentUser.EmployeeId = 0;
            CurrentUser.Role = (int)CurrentUser.Roles.Unasigned;
        }
    }
}
