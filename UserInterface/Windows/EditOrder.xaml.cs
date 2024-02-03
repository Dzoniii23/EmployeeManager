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
    /// Interaction logic for EditOrder.xaml
    /// </summary>
    public partial class AddOrder : Window
    {
        private OrderReport _orderToEdit;

        public AddOrder(OrderReport orderToEdit)
        {
            InitializeComponent();

            // Store the OrderReport object for later use
            _orderToEdit = orderToEdit;

            InitializeData();

        }
        #region Events
        private async void btnEditItem_Click(object sender, RoutedEventArgs e)
        {
            ProductInOrder selectedProduct = (ProductInOrder)productsList.SelectedItem;

            if (selectedProduct != null)
            {
                EditPart editPartWindow = new EditPart((ProductInOrder)productsList.SelectedItem);

                if (editPartWindow.ShowDialog() == false && editPartWindow._returnVal != null)
                {
                    await UpdateOrderWithProduct(editPartWindow._returnVal);
                }
            }
            else
            {
                MessageBox.Show("Select the product to edit first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            EditPart editPartWindow = new EditPart(null);

            if (editPartWindow.ShowDialog() == false && editPartWindow._returnVal != null)
            {
                await UpdateOrderWithProduct(editPartWindow._returnVal);
            }
        }
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of ApiService
            var apiService = new ApiService();

            // Create query for controller
            OrderEditReport orderEditReportQuery = new OrderEditReport { OrderId = _orderToEdit.OrderId };

            // Get order detail data from DB
            var orderDetails = await apiService.GetOrderDetailAsync(orderEditReportQuery);

            orderDetails.CustId = int.Parse(txtCustId.Text);
            orderDetails.ShipName = txtShipToName.Text;
            orderDetails.ShipAddress = txtShipToAddress.Text;
            orderDetails.ShipCity = txtShipToCity.Text;
            orderDetails.ShipRegion = txtShipToRegion.Text;
            orderDetails.ShipPostalCode = txtShipToPostalCode.Text;
            orderDetails.ShipCountry = txtShipToCountry.Text;
            orderDetails.RequiredDate = DateTime.Parse(dpRequiredDate.Text);

            // Update the order 
            try
            {
                var result = await apiService.UpdateOrderAsync(orderDetails);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order: {ex.Message}");
            }

            InitializeData();
        }
        private async void btnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            ProductInOrder selectedProduct = (ProductInOrder)productsList.SelectedItem;

            if (selectedProduct != null )
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this product from the order? This action cannot be undone.",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Create an instance of ApiService
                    var apiService = new ApiService();

                    // Delete the part
                    var resultOfDeletion = await apiService.DeleteProductFromOrder(_orderToEdit.OrderId, selectedProduct.ProductId);

                    // Refresh the screen
                    if (resultOfDeletion) 
                    {
                        InitializeData();
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
        private async void InitializeData()
        {
            try
            {
                // Create an instance of ApiService
                var apiService = new ApiService();

                // Create query for controller
                OrderEditReport orderEditReportQuery = new OrderEditReport { OrderId = _orderToEdit.OrderId };

                //Get order detail data from DB
                var orderDetails = await apiService.GetOrderDetailAsync(orderEditReportQuery);

                //Populate fields
                PopulateFileds(orderDetails);

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
        private void PopulateFileds(OrderEditReport order)
        {
            txtCustId.Text = order.CustId.ToString();
            txtCompanyName.Text = order.CompanyName;
            txtContactName.Text = order.ContactName;
            txtCity.Text = order.City;
            txtPostalCode.Text = order.PostalCode;
            txtRegion.Text = order.Region;
            txtCountry.Text = order.Country;

            txtShipToName.Text = order.ShipName;
            txtShipToAddress.Text = order.ShipAddress;
            txtShipToCity.Text = order.ShipCity;
            txtShipToRegion.Text = order.ShipRegion;
            txtShipToPostalCode.Text = order.ShipPostalCode;
            txtShipToCountry.Text = order.ShipCountry;

            txtOrderId.Text = order.OrderId.ToString();
            txtEmployeeId.Text = order.EmpId.ToString();
            OrderQueryModel.OrderState orderState = (OrderQueryModel.OrderState)order.State;
            txtOrderState.Text = orderState.ToString();
            txtOrderDate.Text = order.OrderDate.ToString("d");
            dpRequiredDate.Text = order.RequiredDate.ToString("d");
            txtShippedDate.Text = order.ShippedDate.ToString();

            productsList.DataContext = order;
            productsList.ItemsSource = order.ProductsInOrder;
        }
        private async Task UpdateOrderWithProduct(ProductInOrder product)
        {
            try
            {
                // Create an instance of ApiService
                var apiService = new ApiService();

                // Create query for controller
                OrderEditReport orderEditReportQuery = new OrderEditReport { OrderId = _orderToEdit.OrderId };

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

                InitializeData();

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

        #endregion

        
    }
}
