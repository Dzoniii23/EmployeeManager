using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Globalization;

namespace UserInterface.Windows
{
    /// <summary>
    /// Interaction logic for EditPart.xaml
    /// </summary>
    public partial class EditPart : Window
    {
        private ProductInOrder? _initalProduct;
        private List<ProductToAdd> _productList;
        private List<int> _productIds;
        public ProductInOrder? _returnVal = new ProductInOrder();

        public EditPart(ProductInOrder? product)
        {
            InitializeComponent();

            // Store the ProductInOrder object for later use
            _initalProduct = product;

            try
            {
                //Get the list of available products
                LoadProducts();
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Unable to get the list of available products!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (result == MessageBoxResult.OK)
                {
                    Close();
                }
            }
                        
            if (_initalProduct != null )
            {
                //Initialize text boxes with selected item
                cbProductIds.SelectedValue = _initalProduct.ProductId;
            }
            else
            {
                //Initialize text boxes with first product from product list
                cbProductIds.SelectedIndex = 0;
            }
        }

        #region Events
        private void cbProductIds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //If selection changed refresh the textboxes with selected product value
            UpdateTextBoxes(product: _productList.FirstOrDefault(x => x.ProductId == (int)cbProductIds.SelectedValue));
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //If user clicks save store currently selected data to returnVal and close the window
            _returnVal.ProductId = (int)cbProductIds.SelectedItem;
            _returnVal.ProductName = txtProductName.Text;
            _returnVal.UnitPrice = Decimal.Parse(txtProductUnitPrice.Text, NumberStyles.AllowDecimalPoint);
            _returnVal.Stock = Decimal.Parse(txtProductStock.Text, NumberStyles.AllowDecimalPoint);
            _returnVal.Discontinued = Boolean.Parse(txtProductDiscontinued.Text);
            _returnVal.Qty = short.Parse(txtProductQty.Text, NumberStyles.Integer);
            _returnVal.Discount = Decimal.Parse(txtProductDiscount.Text, NumberStyles.AllowDecimalPoint) / 100;

            Close();
        }
        #endregion

        #region Helper methods
        private async void LoadProducts()
        {
            //Create api service
            ApiService apiService = new ApiService();

            //Populate list of products
            _productList = await apiService.GetProductsListAsync();

            //Populate list of product Ids
            _productIds = _productList.Select(x => x.ProductId).ToList();

            //Use product Ids list as a source for drop down
            cbProductIds.ItemsSource = _productIds;
        }

        private void UpdateTextBoxes(ProductToAdd product)
        {
            // Update text boxes based on the selected product
            txtProductName.Text = product.ProductName;
            txtProductUnitPrice.Text = product.UnitPrice.ToString();
            txtProductStock.Text = product.Stock.ToString();
            txtProductDiscontinued.Text = product.Discontinued.ToString();

            if (_initalProduct != null)
            {
                if (_initalProduct.ProductId == product.ProductId)
                {
                    txtProductQty.Text = _initalProduct.Qty.ToString();
                    txtProductDiscount.Text = (_initalProduct.Discount * 100).ToString();
                }
                else
                {
                    // Reset to 0 when a different product than the one already in order is selected
                    txtProductQty.Text = "0";
                    txtProductDiscount.Text = "0";
                }
            }
            else
            {
                // Reset to 0 when a different product than the one already in order is selected
                txtProductQty.Text = "0";
                txtProductDiscount.Text = "0";
            }
        }
        #endregion
    }
}
