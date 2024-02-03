using System;
using System.Threading.Tasks;
using Web.Models;
using SharedLibrary;
using Microsoft.EntityFrameworkCore;
using static Web.Models.DbTables;
using Microsoft.AspNetCore.Mvc;

namespace Web.Services
{
    public class OrderService
    {
        private readonly ApplicationContext _context;

        public OrderService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<OrderReport>>> GetOrdersAsync(OrderQueryModel orderQuery)
        {
            var query = from order in _context.Orders
                        join customer in _context.Customers on order.CustId equals customer.CustId
                        join employee in _context.Employees on order.EmpId equals employee.EmpId
                        join orderDetail in _context.OrderDetails on order.OrderId equals orderDetail.OrderId
                        where
                            (!orderQuery.EmpId.HasValue || order.EmpId == orderQuery.EmpId) &&
                            (!orderQuery.CustId.HasValue || order.CustId == orderQuery.CustId) &&
                            (string.IsNullOrEmpty(orderQuery.CompanyName) || customer.CompanyName.Contains(orderQuery.CompanyName)) &&
                            (string.IsNullOrEmpty(orderQuery.City) || customer.City.Contains(orderQuery.City)) &&
                            (string.IsNullOrEmpty(orderQuery.Region) || customer.Region.Contains(orderQuery.Region)) &&
                            (string.IsNullOrEmpty(orderQuery.Country) || customer.Country.Contains(orderQuery.Country)) &&
                            (!orderQuery.OrderDateFrom.HasValue || order.OrderDate.Date >= orderQuery.OrderDateFrom.Value.Date) &&
                            (!orderQuery.OrderDateTo.HasValue || order.OrderDate.Date <= orderQuery.OrderDateTo.Value.Date) &&
                            (!orderQuery.RequiredDateFrom.HasValue || order.RequiredDate.Date >= orderQuery.RequiredDateFrom.Value.Date) &&
                            (!orderQuery.RequiredDateTo.HasValue || order.RequiredDate.Date <= orderQuery.RequiredDateTo.Value.Date) &&
                            (!orderQuery.State.HasValue || order.State == (int)orderQuery.State)
                        group orderDetail by new
                        {
                            order.EmpId,
                            order.OrderId,
                            order.CustId,
                            customer.CompanyName,
                            customer.City,
                            customer.Region,
                            customer.Country,
                            order.OrderDate,
                            order.RequiredDate,
                            order.State
                        } into grouped
                        select new OrderReport
                        {
                            EmpId = grouped.Key.EmpId,
                            EmpName = String.Format("{0} {1}",
                                                    _context.Employees.FirstOrDefault(e => e.EmpId == grouped.Key.EmpId).FirstName,
                                                    _context.Employees.FirstOrDefault(e => e.EmpId == grouped.Key.EmpId).LastName),
                            OrderId = grouped.Key.OrderId,
                            CustId = (int)grouped.Key.CustId,
                            CompanyName = grouped.Key.CompanyName,
                            City = grouped.Key.City,
                            Region = grouped.Key.Region,
                            Country = grouped.Key.Country,
                            OrderDate = grouped.Key.OrderDate,
                            RequiredDate = grouped.Key.RequiredDate,
                            State = grouped.Key.State
                            //State = grouped.Sum(od => od.State)
                        };

            var orderReports = await query.ToListAsync();

            return orderReports;
        }

        public async Task<OrderEditReport> GetOrderDetailsAsync(OrderEditReport order)
        {
            // Fetch order details
            var selectedOrderDetails = await _context.Orders.FindAsync(order.OrderId);
            if (selectedOrderDetails == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            // Fetch customer details
            var customer = await _context.Customers.FindAsync(selectedOrderDetails.CustId);
            if (customer == null)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            // Fetch employee details
            var employee = await _context.Employees.FindAsync(selectedOrderDetails.EmpId);
            if (employee == null)
            {
                throw new InvalidOperationException("Employee not found.");
            }

            //Fetch  product list
            List<ProductInOrder> productsInOrder = await GetProductsInOrder(order);

            // Create and return OrderEditReport
            return new OrderEditReport
            {
                OrderId = selectedOrderDetails.OrderId,
                State = selectedOrderDetails.State,
                OrderDate = selectedOrderDetails.OrderDate,
                RequiredDate = selectedOrderDetails.RequiredDate,
                ShippedDate = selectedOrderDetails.ShippedDate,
                EmpId = selectedOrderDetails.EmpId,
                CustId = selectedOrderDetails.CustId,
                CompanyName = customer.CompanyName,
                ContactName = customer.ContactName,
                Address = customer.Address,
                City = customer.City,
                Region = customer.Region,
                PostalCode = customer.PostalCode,
                Country = customer.Country,
                ShipName = selectedOrderDetails.ShipName,
                ShipAddress = selectedOrderDetails.ShipAddress,
                ShipCity = selectedOrderDetails.ShipCity,
                ShipRegion = selectedOrderDetails.ShipRegion,
                ShipPostalCode = selectedOrderDetails.ShipPostalCode,
                ShipCountry = selectedOrderDetails.ShipCountry,
                ProductsInOrder = productsInOrder
            };
        }

        public async Task<bool> UpdateOrderAsync(OrderEditReport order)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Step 1: Update order details
                    var currentOrderData = await _context.Orders.FindAsync(order.OrderId);
                    if (currentOrderData == null)
                    {
                        await transaction.RollbackAsync();
                        throw new InvalidOperationException("Order not found.");
                    }

                    // Update order details
                    currentOrderData.State = order.State;
                    currentOrderData.CustId = order.CustId;
                    currentOrderData.OrderDate = order.OrderDate;
                    currentOrderData.RequiredDate = order.RequiredDate;
                    currentOrderData.ShipName = order.ShipName;
                    currentOrderData.ShipAddress = order.ShipAddress;
                    currentOrderData.ShipCity = order.ShipCity;
                    currentOrderData.ShipRegion = order.ShipRegion;
                    currentOrderData.ShipPostalCode = order.ShipPostalCode;
                    currentOrderData.ShipCountry = order.ShipCountry;
                    await _context.SaveChangesAsync();

                    // Step 2: Update or add new products to the order
                    await UpdateOrderDetailsAsync(order);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an exception
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<int> CreateOrderAsync(OrderEditReport order)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (order.OrderId != 0)
                    {
                        //Update existing order
                        await UpdateOrderAsync(order);
                        return order.OrderId;
                    }

                    //Create new order
                    var newOrder = new Order
                    {
                        // Let the database generate OrderId because it is configured as an identity column
                        // Populate remaining columns
                        State = order.State,
                        CustId = order.CustId,
                        EmpId = order.EmpId,
                        OrderDate = order.OrderDate,
                        RequiredDate = order.RequiredDate,
                        ShippedDate = order.ShippedDate,
                        ShipperId = 1,
                        Freight = 0,
                        ShipName = order.ShipName,
                        ShipAddress = order.ShipAddress,
                        ShipCity = order.ShipCity,
                        ShipRegion = order.ShipRegion,
                        ShipPostalCode = order.ShipPostalCode,
                        ShipCountry = order.ShipCountry
                    };                    

                    _context.Orders.Add(newOrder);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Reload the newOrder entity to get the updated OrderId
                    _context.Entry(newOrder).Reload();

                    if (order.ProductsInOrder != null)
                    {
                        OrderEditReport tempOrder = order;
                        tempOrder.OrderId = newOrder.OrderId;

                        // Update order details
                        await UpdateOrderDetailsAsync(tempOrder);

                        // Save changes to the database
                        await _context.SaveChangesAsync();
                    }

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return newOrder.OrderId;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an exception
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<bool> DeleteProductFromOrder(int orderId, int productId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Get part to remove
                    var orderDetail = await _context.OrderDetails.FindAsync(orderId, productId);
                    if (orderDetail == null)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("Product not found.");
                    }

                    var productDetail = await _context.Products.FindAsync(productId);
                    if (productDetail == null)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("Product not found.");
                    }

                    //Restock the product
                    productDetail.Stock += orderDetail.Qty;

                    // Remove part
                    _context.OrderDetails.Remove(orderDetail);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an exception
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<int> DeleteOrderAsync(OrderEditReport order)
        {
            var orderToDelete = await _context.Orders.FindAsync(order.OrderId);

            if (orderToDelete == null)
            {
                return 1; // Order not found
            }

            if (orderToDelete.State == (int)SharedLibrary.OrderQueryModel.OrderState.Shipped)
            {
                return 2; // Cannot delete a shipped order
            }

            //Fetch  product list
            List<ProductInOrder> productsInOrder = await GetProductsInOrder(order);

            //Check if there are any products in the order
            if (productsInOrder != null)
            {
                foreach (var productInOrder in productsInOrder)
                {
                    var productToRestock = await _context.Products.FindAsync(productInOrder.ProductId);

                    if (productToRestock != null)
                    {
                        // Restock the product
                        productToRestock.Stock += productInOrder.Qty;
                    }
                }
            }

            //Remove all products from Sales.OrderDetails table
            var orderDetailsToRemove = _context.OrderDetails.Where(od => od.OrderId == orderToDelete.OrderId);
            _context.OrderDetails.RemoveRange(orderDetailsToRemove);

            //Remove order from Sales.Orders table
            _context.Orders.Remove(orderToDelete);

            //Save changes in database
            await _context.SaveChangesAsync();

            return 0; // Successfully deleted
        }
        
        public async Task<List<ProductToAdd>> GetProductsForDropdownAsync()
        {
            // Retrieve only products with non-zero stock from the database
            //var products = await _context.Products.Where(p => !p.Discontinued && p.Stock > 0m).ToListAsync();
            var products = _context.Products;//.Where(p => p.Stock > 0).ToList(); //!p.Discontinued && 

            // Map the products to ProductToAdd class
            var productsToAdd = products.Select(p => new ProductToAdd
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = p.UnitPrice,
                Discontinued = p.Discontinued,
                Stock = p.Stock
            }).ToList();

            return productsToAdd;
        }

        #region Helper methods
        private async Task<List<ProductInOrder>> GetProductsInOrder(OrderEditReport order)
        {
            return await (from orderDetail in _context.OrderDetails
                          join product in _context.Products on orderDetail.ProductId equals product.ProductId
                          where orderDetail.OrderId == order.OrderId
                          select new ProductInOrder
                          {
                              ProductId = product.ProductId,
                              ProductName = product.ProductName,
                              UnitPrice = product.UnitPrice,
                              Qty = orderDetail.Qty,
                              Discount = orderDetail.Discount,
                              Discontinued = product.Discontinued,
                              Stock = product.Stock
                          }).ToListAsync();
        }

        private OrderEditReport? MapDataToOrderEditReport(int orderId)
        {
            var orderDetails = (
                from order in _context.Orders
                join customer in _context.Customers on order.CustId equals customer.CustId
                join employee in _context.Employees on order.EmpId equals employee.EmpId
                where order.OrderId == orderId
                select new
                {
                    Order = order,
                    Customer = customer,
                    Employee = employee
                }
            ).FirstOrDefault();

            if ( orderDetails != null )
            {
                // Handle the case where the order with the given ID is not found
                return null;
            }

            var orderEditReport = new OrderEditReport
            {
                OrderId = orderDetails.Order.OrderId,
                State = orderDetails.Order.State,
                OrderDate = orderDetails.Order.OrderDate,
                RequiredDate = orderDetails.Order.RequiredDate,
                ShippedDate = orderDetails.Order.ShippedDate,
                EmpId = orderDetails.Order.EmpId,

                CustId = orderDetails.Customer.CustId,
                CompanyName = orderDetails.Customer.CompanyName,
                ContactName = orderDetails.Customer.ContactName,
                Address = orderDetails.Customer.Address,
                City = orderDetails.Customer.City,
                Region = orderDetails.Customer.Region,
                PostalCode = orderDetails.Customer.PostalCode,
                Country = orderDetails.Customer.Country,

                ShipName = orderDetails.Order.ShipName,
                ShipAddress = orderDetails.Order.ShipAddress,
                ShipCity = orderDetails.Order.ShipCity,
                ShipRegion = orderDetails.Order.ShipRegion,
                ShipPostalCode = orderDetails.Order.ShipPostalCode,
                ShipCountry = orderDetails.Order.ShipCountry,

                ProductsInOrder = MapProductsInOrder(orderId)
            };

            return orderEditReport;
        }

        private List<ProductInOrder> MapProductsInOrder(int orderId)
        {

            var productsInOrder = (
                from orderDetail in _context.OrderDetails
                join product in _context.Products on orderDetail.ProductId equals product.ProductId
                where orderDetail.OrderId == orderId
                select new ProductInOrder
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = orderDetail.UnitPrice,
                    Qty = orderDetail.Qty,
                    Discount = orderDetail.Discount,
                    Discontinued = product.Discontinued,
                    Stock = product.Stock
                }).ToList();

            return productsInOrder;
        }

        private async Task<decimal> GetUnitPriceAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            return product?.UnitPrice ?? 0; // Default to 0 if product or UnitPrice is not found
        }

        private bool HasEnoughStock(int productId, short requiredQty)
        {
            var product = _context.Products.Find(productId);

            // Check if there is enough stock
            return product?.Stock >= requiredQty;
        }

        private async Task UpdateOrderDetailsAsync(OrderEditReport order)
        {
            foreach (var product in order.ProductsInOrder)
            {
                // Retrieve the current order details for the specific product
                OrderDetail? selectedProduct = await _context.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderId == order.OrderId && od.ProductId == product.ProductId);

                if (selectedProduct != null)
                {
                    // Product is existing in the order already, and we need to alter its details
                    await UpdateExistingProductInOrderAsync(order, product);

                }
                else
                {
                    // Product is not existing in the order already, so we need to add it to the order
                    await AddNewProductToOrderAsync(order, product);
                }
            }
        }
        
        private async Task UpdateExistingProductInOrderAsync(OrderEditReport order, ProductInOrder selectedProduct)
        {
            // Fetch product details from Production.Products table
            Product? productDetails = await _context.Products.FindAsync(selectedProduct.ProductId);

            //Fetch item detail from Sales.OrderDetails table
            var item = await _context.OrderDetails
                    .Where(p => p.OrderId == order.OrderId && p.ProductId == selectedProduct.ProductId)
                    .FirstOrDefaultAsync();

            if (productDetails != null)
            {
                // If the product exists in Production.Products table

                if (selectedProduct.Qty > item.Qty && productDetails.Stock >= selectedProduct.Qty - item.Qty)
                {
                    //Reduce the amount of stock
                    productDetails.Stock -= selectedProduct.Qty - item.Qty;

                    //Update quantity of item order
                    item.Qty = selectedProduct.Qty;
                }

                if (selectedProduct.Qty < item.Qty)
                {
                    //Increase the amount of stock
                    productDetails.Stock += item.Qty - selectedProduct.Qty;

                    //Update quantity of item order
                    item.Qty = selectedProduct.Qty;
                }

                //Check if discount if different
                if (selectedProduct.Discount != item.Discount)
                {
                    item.Discount = selectedProduct.Discount;
                }
            }
            else
            {
                throw new InvalidOperationException($"Product {selectedProduct.ProductId} not found.");
            }
        }
        
        private async Task AddNewProductToOrderAsync(OrderEditReport order, ProductInOrder selectedProduct)
        {
            // Fetch product details from Production.Products table
            var productDetail = await _context.Products.FindAsync(selectedProduct.ProductId);

            if (productDetail != null)
            {
                if (selectedProduct.Qty <= productDetail.Stock)
                {
                    // Add new item to the Sales.OrderDetails table
                    var newItem = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = productDetail.ProductId,
                        UnitPrice = productDetail.UnitPrice,
                        Qty = selectedProduct.Qty,
                        Discount = selectedProduct.Discount
                    };
                    _context.OrderDetails.Add(newItem);

                    //Reduce amount of stock in Production.Products table
                    productDetail.Stock -= selectedProduct.Qty;
                }
                else
                {
                    throw new InvalidOperationException($"Not enough {selectedProduct.ProductId} on stock.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Product {selectedProduct.ProductId} not found.");
            }
        }
        #endregion
    }
}
