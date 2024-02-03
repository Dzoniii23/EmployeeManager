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
using SharedLibrary;
using UserInterface.Services;

namespace UserInterface.Windows
{
    /// <summary>
    /// Interaction logic for UpdateCustomer.xaml
    /// </summary>
    public partial class UpdateCustomer : Window
    {
        private int _mode;
        private List<CustomerReport> _customersList;
        private List<int> _customerIdsList;

        public UpdateCustomer(int Mode)
        {
            InitializeComponent();
            //Store mode for later use
            _mode = Mode;

            //Initalize screen based on selected mode
            if (_mode == 0) 
            { 
                cbCustId.IsEnabled = false; 
            } 
            else 
            {  
                cbCustId.IsEnabled = true;
                Refresh(1);
            }

            
        }
        #region Helper methods
        private async void AddCustomer()
        {
            //Collect data from txt fields
            CustomerReport tempCustomer = new CustomerReport
            {
                CustId = 0,
                ContactName = txtContactname.Text,
                CompanyName = txtCompanyName.Text,
                ContactTitle = txtContactTitle.Text,
                Address = txtAddress.Text,
                City = txtCity.Text,
                Region = txtRegion.Text,
                PostalCode = txtPostalCode.Text,
                Country = txtCountry.Text,
                Phone = txtPhone.Text,
                Fax = txtFax.Text
            };

            try
            {
                //Create api service
                ApiService apiService = new ApiService();

                //Execute api service
                var response = await apiService.AddCustomer(tempCustomer);

                if (response == -1)
                {
                    MessageBox.Show("Failed to edit customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                MessageBoxResult boxResult = MessageBox.Show(String.Format("Succesfully created customer. New customer ID: {0}", response.ToString()),
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);

                if (boxResult == MessageBoxResult.OK)
                {
                    Close();
                }
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Unable to update the customers!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }

        private async void EditCustomer()
        {
            //Collect data from txt fields
            CustomerReport tempCustomer = new CustomerReport
            {
                CustId = (int)cbCustId.SelectedValue,
                CompanyName = txtCompanyName.Text,
                ContactName = txtContactname.Text,
                ContactTitle = txtContactTitle.Text,
                Address = txtAddress.Text,
                City = txtCity.Text,
                Region = txtRegion.Text,
                PostalCode = txtPostalCode.Text,
                Country = txtCountry.Text,
                Phone = txtPhone.Text,
                Fax = txtFax.Text
            };

            try
            {
                //Create api service
                ApiService apiService = new ApiService();

                //Execute api service
                var response = await apiService.UpdateCustomer(tempCustomer);

                switch (response)
                {
                    case -1: //Customer is null
                        MessageBox.Show("Failed to edit customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;

                    case -2: //Customer not found in database
                        MessageBoxResult result = MessageBox.Show("Customer not found in database!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            Close();
                        }
                        break;

                    case 1: //Success
                        Refresh(tempCustomer.CustId);
                        break;
                }
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Unable to update the customers!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }

        private async void Refresh(int? @int)
        {
            try
            {
                //Create api service
                ApiService apiService = new ApiService();

                //Populate list of customers
                _customersList = await apiService.GetListOfCustomers();

                //Populate list of customer ids
                _customerIdsList = _customersList.Select(x => x.CustId).ToList();

                //Use customer ids list as a source for drop down
                cbCustId.ItemsSource = _customerIdsList;
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Unable to get the list of available customers!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    Close();
                }
            }

            if (@int != null)
            {
                //Trigger cbCustId_SelectionChanged
                cbCustId.SelectedValue = @int;
            }
        }
        #endregion
        #region Events
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            switch (_mode)
            {
                case 0: //Add new cusomer
                    AddCustomer();
                    break;

                case 1: //Edit existing customer
                    EditCustomer();
                    break;
            }
        }
        private void cbCustId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCustId.SelectedItem != null)
            {
                var tempCustomer = _customersList.Find(x => x.CustId == (int)cbCustId.SelectedValue);
                if (tempCustomer != null)
                {
                    txtCompanyName.Text = tempCustomer.CompanyName;
                    txtContactname.Text = tempCustomer.ContactName;
                    txtContactTitle.Text = tempCustomer.ContactTitle;
                    txtAddress.Text = tempCustomer.Address;
                    txtCity.Text = tempCustomer.City;
                    txtRegion.Text = tempCustomer.Region;
                    txtPostalCode.Text = tempCustomer.PostalCode;
                    txtCountry.Text = tempCustomer.Country;
                    txtPhone.Text = tempCustomer.Phone;
                    txtFax.Text = tempCustomer.Fax;
                }
            }
        }
        #endregion
    }
}
