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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserInterface.Services;
using UserInterface.Windows;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;


namespace UserInterface.Views
{
    /// <summary>
    /// Interaction logic for ViewSales.xaml
    /// </summary>
    public partial class ViewSales : UserControl
    {
        #region Constructor
        public ViewSales()
        {
            InitializeComponent();

            var orderQuery = new OrderQueryModel();
            
            switch (CurrentUser.Role)
            {
                case (int)CurrentUser.Roles.Sales:
                    orderQuery.EmpId = CurrentUser.EmployeeId;
                    break;

                case (int)CurrentUser.Roles.SalesMngr:
                    orderQuery.EmpId = null;
                    break;
            }            

            Refresh(orderQuery);
        }
        #endregion

        #region Events
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OrderReport selectedOrder = (OrderReport)reportData.SelectedItem;

            if (selectedOrder != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this order? This action cannot be undone.",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Create an instance of ApiService
                    var apiService = new ApiService();

                    // Create query for controller
                    OrderEditReport orderEditReportQuery = new OrderEditReport { OrderId = selectedOrder.OrderId };

                    //Get order detail data from DB
                    var orderDetails = await apiService.GetOrderDetailAsync(orderEditReportQuery);

                    // Delete the order
                    var resultOfDeletion = await apiService.DeleteOrder(orderDetails);

                    if (resultOfDeletion)
                    {
                        //Refresh the screen
                        var orderQuery = new OrderQueryModel
                        {
                            EmpId = CurrentUser.EmployeeId, // Set employee id
                        };
                        Refresh(orderQuery);
                    }
                }

            }
            else
            {
                MessageBox.Show("Select the order to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item from reportData
            OrderReport selectedOrder = (OrderReport)reportData.SelectedItem;

            if (selectedOrder != null)
            {
                AddOrder editOrderWindow = new AddOrder(selectedOrder);

                if (editOrderWindow.ShowDialog() == false)
                {
                    // Edit order window is closed

                    //Set query for refreshing the list
                    OrderQueryModel orderQuery = new OrderQueryModel {EmpId = CurrentUser.EmployeeId};
                    //Refresh the list
                    Refresh(orderQuery);
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select an order to edit.", 
                    "Edit Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Exclamation);
            }
        }
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterOrders filterOrders = new();

            if (filterOrders.ShowDialog() == false)
            {
                Refresh(filterOrders.FilterCriteria);
            }
        }
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //Setting License Context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                //Creating Excel Package
                using (var excelPackage = new ExcelPackage())
                {
                    //Creating Worksheet
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                    /* 
                    Add headers to the worksheet 
                    This part retrieves the GridView from reportData and adds the column headers to the first row of the Excel worksheet.
                    */
                    var gridView = reportData.View as GridView;
                    if (gridView != null)
                    {
                        for (int col = 1; col <= gridView.Columns.Count; col++)
                        {
                            worksheet.Cells[1, col].Value = gridView.Columns[col - 1].Header;
                        }
                    }

                    /*
                    Exporting Data to Excel
                    This part iterates over the rows and columns of reportData and extracts data from each cell. 
                    It uses reflection to get the property value based on the property name derived from the binding's path. 
                    */
                    for (int row = 0; row < reportData.Items.Count; row++) //This loop iterates through each row of the reportData(assumed to be a ListView).
                    {
                        for (int col = 0; col < gridView.Columns.Count; col++) //This loop iterates through each column of the GridView associated with the reportData.
                        {
                            /*
                            Getting Display Member Binding
                            Retriving the DisplayMemberBinding of the current column in the GridView. 
                            This represents the binding between the data source and the visual representation. 
                            */
                            var bindingBase = gridView.Columns[col].DisplayMemberBinding;

                            if (bindingBase is Binding binding) //Check if the DisplayMemberBinding is actually a Binding. This is important because not all types of bindings may be used in this context.
                            {
                                var propertyName = binding.Path.Path; //Retriving the property name from the binding's path. Assuming that the DisplayMemberBinding is a simple property path.

                                //Using reflection to get the value of the property (propertyName) from the corresponding item in the reportData.Items. Assuming that the items are of type OrderReport.
                                var cellValue = ((OrderReport)reportData.Items[row]).GetType().GetProperty(propertyName)?.GetValue(reportData.Items[row], null);

                                // Now we can use cellValue to set the cell value in Excel
                                worksheet.Cells[row + 2, col + 1].Value = cellValue;
                            }
                        }
                    }

                    /*
                    Auto fit columns for better visibility
                    This line automatically adjusts the width of the columns in the Excel worksheet for better visibility.
                    */
                    worksheet.Cells.AutoFitColumns();

                    /*
                    Save the Excel package to a file
                    This part opens a save file dialog, allowing the user to choose where to save the Excel file.
                    If the user selects a location, the Excel file is saved, and a success message is displayed.
                    */
                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                        DefaultExt = "xlsx"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        FileInfo excelFile = new FileInfo(saveFileDialog.FileName);
                        excelPackage.SaveAs(excelFile);
                        MessageBox.Show("Data exported successfully.", "Export Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                /*
                Handling Exceptions
                This catch block handles any exceptions that might occur during the export process and displays an error message. 
                */
                MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddNewOrder addNewOrder = new AddNewOrder();

            if (addNewOrder.ShowDialog() == false)
            {
                //Set query for refreshing the list
                OrderQueryModel orderQuery = new OrderQueryModel { EmpId = CurrentUser.EmployeeId };
                //Refresh the list
                Refresh(orderQuery);
            }
        }
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            UpdateCustomer updateCustomer = new(0);
            if (updateCustomer.ShowDialog() == false)
            {
                return;
            }
        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            UpdateCustomer updateCustomer = new(1);
            if (updateCustomer.ShowDialog() == false)
            {
                return;
            }
        }
        #endregion
        #region Refresh
        public async void Refresh(OrderQueryModel orderQuery)
        {
            try
            {
                // Create an instance of ApiService
                var apiService = new ApiService();

                var orders = await apiService.GetOrdersAsync(orderQuery);
                
                reportData.DataContext = orders;
                reportData.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., log or show an error message)
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        #endregion

    }
}
