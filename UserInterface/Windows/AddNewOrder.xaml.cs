using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
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
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;
using SharedLibrary;
using UserInterface.Services;

namespace UserInterface.Windows
{
    /// <summary>
    /// Interaction logic for AddNewOrder.xaml
    /// </summary>
    public partial class AddNewOrder : Window
    {
        private OrderEditReport _orderToAdd;
        private List<CustomerReport> _customersList;
        private List<int> _customerIdsList;

        #region Constructor
        public AddNewOrder()
        {
            InitializeComponent();
            _orderToAdd = new OrderEditReport();
            InitializeOrderData();

        }
        #endregion

        #region Events
        private void cbCustId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCustId.SelectedValue != null)
            {
                _orderToAdd.CustId = (int)cbCustId.SelectedValue;
                var tempCustomer = _customersList.Find(x => x.CustId == _orderToAdd.CustId);
                if (tempCustomer != null)
                {
                    _orderToAdd.CompanyName = tempCustomer.CompanyName;
                    _orderToAdd.ContactName = tempCustomer.ContactName;
                    _orderToAdd.City = tempCustomer.City;
                    _orderToAdd.PostalCode = tempCustomer.PostalCode;
                    _orderToAdd.Region = tempCustomer.Region;
                    _orderToAdd.Country = tempCustomer.Country;
                }

                RefreshCustomerDetails();
            }            
        }
        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_orderToAdd.OrderId == 0)
            {
                MessageBoxResult result = MessageBox.Show("Befor you add new part you need firstly to create an order by saving the order details!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            EditPart editPartWindow = new EditPart(null);

            if (editPartWindow.ShowDialog() == false && editPartWindow._returnVal != null)
            {
                await UpdateOrderWithProduct(editPartWindow._returnVal);
                RefreshAll();
            }
        }
        private async void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            ProductInOrder selectedProduct = (ProductInOrder)productsList.SelectedItem;

            if (_orderToAdd.OrderId == 0)
            {
                MessageBoxResult result = MessageBox.Show("Befor you edit the part you need firstly to create an order by saving the order details!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            if (selectedProduct != null)
            {
                EditPart editPartWindow = new EditPart((ProductInOrder)productsList.SelectedItem);

                if (editPartWindow.ShowDialog() == false && editPartWindow._returnVal != null)
                {
                    await UpdateOrderWithProduct(editPartWindow._returnVal);
                    RefreshAll();
                }
            }
            else
            {
                MessageBox.Show("Select the product to edit first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrderData();

            if (string.Equals(_orderToAdd.OrderDate.ToString("d"), _orderToAdd.RequiredDate.ToString("d")))
            {
                MessageBox.Show("Required date cannot be the same as order date!", 
                    "Change required date", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            try
            {
                // Create an instance of ApiService
                var apiService = new ApiService();

                int response = await apiService.CreateNewOrder(_orderToAdd);

                if (response == 0)
                {
                    MessageBox.Show("Failed to save order", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }

                _orderToAdd.OrderId = response;
                RefreshAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving order: {ex.Message}");
            }
        }
        private async void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            ProductInOrder selectedProduct = (ProductInOrder)productsList.SelectedItem;

            if (_orderToAdd.OrderId == 0)
            {
                MessageBoxResult result = MessageBox.Show("Prior to part removal you need firstly to create an order by saving the order details!",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (result == MessageBoxResult.OK)
                {
                    return;
                }
            }

            if (selectedProduct != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this product from the order? This action cannot be undone.",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Create an instance of ApiService
                    var apiService = new ApiService();

                    // Delete the part
                    var resultOfDeletion = await apiService.DeleteProductFromOrder(_orderToAdd.OrderId, selectedProduct.ProductId);

                    // Refresh the screen
                    if (resultOfDeletion)
                    {
                        RefreshAll();
                    }
                }
            }
            else
            {
                MessageBox.Show("Select the product to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        #region Helper methods
        private async void InitializeOrderData()
        {
            try
            {
                //Create api service
                ApiService apiService = new ApiService();

                //Populate list of customers
                _customersList = await apiService.GetListOfCustomers();

                //Populate list of customer ids
                _customerIdsList = _customersList.Select(x => x.CustId).ToList();

                //Use customer ids lis as a source for drop down
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
            _orderToAdd.EmpId = CurrentUser.EmployeeId;
            _orderToAdd.OrderDate = DateTime.Now;
            _orderToAdd.RequiredDate = DateTime.Now;
            _orderToAdd.State = (int)OrderQueryModel.OrderState.Unknown;
            
            RefreshCustomerDetails();
            RefreshShipToDetails();
            RefreshOrderDetails();
            RefreshProductList();
        }
        private void RefreshCustomerDetails()
        {
            txtCompanyName.Text = _orderToAdd.CompanyName;
            txtContactName.Text = _orderToAdd.ContactName;
            txtCity.Text = _orderToAdd.City;
            txtPostalCode.Text = _orderToAdd.PostalCode;
            txtRegion.Text = _orderToAdd.Region;
            txtCountry.Text = _orderToAdd.Country;
        }
        private void RefreshShipToDetails()
        {
            txtShipToName.Text= _orderToAdd.ShipName;
            txtShipToAddress.Text= _orderToAdd.ShipAddress;
            txtShipToCity.Text= _orderToAdd.ShipCity;
            txtShipToRegion.Text= _orderToAdd.ShipRegion;
            txtShipToCountry.Text= _orderToAdd.ShipCountry;
        }
        private void RefreshOrderDetails()
        {
            txtOrderId.Text = _orderToAdd.OrderId.ToString();
            txtEmployeeId.Text = _orderToAdd.EmpId.ToString();
            OrderQueryModel.OrderState orderState = (OrderQueryModel.OrderState)_orderToAdd.State;
            txtOrderState.Text = orderState.ToString();
            txtOrderDate.Text = _orderToAdd.OrderDate.ToString("d");
            dpRequiredDate.Text = _orderToAdd.RequiredDate.ToString("d");
            txtShippedDate.Text = _orderToAdd.ShippedDate.ToString();
        }
        private void RefreshProductList()
        {
            productsList.DataContext = _orderToAdd;
            productsList.ItemsSource = _orderToAdd.ProductsInOrder;
        }
        private async void RefreshAll()
        {
            // Create an instance of ApiService
            var apiService = new ApiService();

            // Create query for controller
            OrderEditReport orderQuery = new OrderEditReport { OrderId = _orderToAdd.OrderId };

            //Get order detail data from DB
            _orderToAdd = await apiService.GetOrderDetailAsync(orderQuery);

            RefreshCustomerDetails();
            RefreshShipToDetails();
            RefreshOrderDetails();
            RefreshProductList();
        }
        private async Task UpdateOrderWithProduct(ProductInOrder product)
        {
            try
            {
                // Create an instance of ApiService
                var apiService = new ApiService();

                // Create query for controller
                OrderEditReport orderEditReportQuery = new OrderEditReport { OrderId = _orderToAdd.OrderId };

                // Get order detail data from DB
                var orderDetails = await apiService.GetOrderDetailAsync(orderEditReportQuery);

                int indexToUpdate = orderDetails.ProductsInOrder.FindIndex(x => x.ProductId == product.ProductId);

                // Check if the item with the specified ProductId is found
                if (indexToUpdate > -1)
                {
                    // Update the item in the list
                    orderDetails.ProductsInOrder[indexToUpdate] = product;
                }
                else
                {
                    // Add new product to the order
                    orderDetails.ProductsInOrder.Add(product);
                }

                // Update the order 
                try
                {
                    var result = await apiService.UpdateOrderAsync(orderDetails);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating order: {ex.Message}");
                }

                RefreshAll();

            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., log or show an error message)
                Console.WriteLine($"Error: {ex.Message}");

                MessageBoxResult result = MessageBox.Show("Error during database connection!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    Close();
                }
            }
        }
        private void UpdateOrderData()
        {
            _orderToAdd.CustId = (int?)cbCustId.SelectedValue;
            _orderToAdd.CompanyName = txtCompanyName.Text;
            _orderToAdd.ContactName = txtContactName.Text;
            _orderToAdd.City = txtCity.Text;
            _orderToAdd.PostalCode = txtPostalCode.Text;
            _orderToAdd.Region = txtRegion.Text;
            _orderToAdd.Country = txtCountry.Text;

            _orderToAdd.ShipName = txtShipToName.Text;
            _orderToAdd.ShipAddress = txtShipToAddress.Text;
            _orderToAdd.ShipCity = txtShipToCity.Text;
            _orderToAdd.ShipRegion = txtShipToRegion.Text;
            _orderToAdd.ShipCountry = txtShipToCountry.Text;

            _orderToAdd.OrderId = int.Parse(txtOrderId.Text);
            _orderToAdd.EmpId = int.Parse(txtEmployeeId.Text);
            if (Enum.TryParse(typeof(OrderQueryModel.OrderState), txtOrderState.Text, out var parsedState))
            {
                _orderToAdd.State = (int)parsedState;
            }            
            _orderToAdd.OrderDate = DateTime.Parse(txtOrderDate.Text);                        
            _orderToAdd.RequiredDate = (DateTime)dpRequiredDate.SelectedDate;
            _orderToAdd.ShippedDate = null;
        }
        #endregion
    }
}
