using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UserInterface.Services;

namespace UserInterface.Windows
{
    /// <summary>
    /// Interaction logic for FilterOrders.xaml
    /// </summary>
    public partial class FilterOrders : Window
    {
        // Property to hold the filter criteria
        public OrderQueryModel FilterCriteria { get; private set; }

        public FilterOrders()
        {
            InitializeComponent();

            txtEmpId.Text = CurrentUser.EmployeeId.ToString();

            if (CurrentUser.Role == (int)CurrentUser.Roles.Sales)
            {
                txtEmpId.IsEnabled = false;
            }
            else
            {
                txtEmpId.IsEnabled = true;
            }

        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Gather user input and create an instance of OrderQueryModel
            FilterCriteria = new OrderQueryModel
            {
                EmpId = ParseInt(txtEmpId.Text),
                CustId = ParseInt(txtCustId.Text),
                CompanyName = txtCompanyName.Text,
                City = txtCity.Text,
                Region = txtRegion.Text,
                Country = txtCountry.Text,
                OrderDateFrom = dpOrderDateFrom.SelectedDate,
                OrderDateTo = dpOrderDateTo.SelectedDate,
                RequiredDateFrom = dpRequiredDateFrom.SelectedDate,
                RequiredDateTo = dpRequiredDateTo.SelectedDate,
                State = (OrderQueryModel.OrderState?)ParseInt(stateComboBox.SelectedValue?.ToString())//(OrderQueryModel.OrderState?)(int?)stateComboBox.SelectedValue
            };

            // Close the window
            Close();
        }

        private int? ParseInt(string input)
        {
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            return null;
        }
    }
}
