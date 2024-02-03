using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UserInterface.Models;
using SharedLibrary;
using System.Windows;
using Newtonsoft.Json;


namespace UserInterface.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7223/");//44335
        }

        public async Task<string> LoginAsync(LoginModel model)
        {
            // Send a POST request to the login endpoint
            var response = await _httpClient.PostAsJsonAsync("api/user/login", model);

            if (response.IsSuccessStatusCode)
            {
                // Return the JWT token and user details from the response
                return await response.Content.ReadAsStringAsync();
            }

            // Handle unsuccessful login
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> ChangePass (ChangePassModel model)
        {
            //Send a POST request to the password endpoint
            var response = await _httpClient.PostAsJsonAsync("api/user/change-password", model);

            if (response.IsSuccessStatusCode)
            {
                // The change password operation was successful
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // The change password operation failed
                var errorMessage = await response.Content.ReadAsStringAsync();
                
                return String.Format("Failed: {0}, errorMessage");
            }
        }
        public async Task<List<OrderReport>> GetOrdersAsync(OrderQueryModel orderQuery)
        {
            try
            {
                // Convert orderQuery to query string
                var queryString = orderQuery.ToQueryString();

                // Build the full URI with query string
                var uri = $"api/order/get-orders{queryString}";

                // Send GET request to the order controller
                var response = await _httpClient.GetFromJsonAsync<List<OrderReport>>(uri);

                if (response != null)
                {
                    return response;
                }

                // Handle unsuccessful response
                return new List<OrderReport>();
            }
            catch (HttpRequestException ex)
            {
                // Handle exception (e.g., log or show an error message)
                Console.WriteLine($"Error: {ex.Message}");
                return new List<OrderReport>();
            }
        }
        public async Task<List<ProductToAdd>> GetProductsListAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ProductToAdd>>("api/order/get-products-for-dropdown");

                if (response != null)
                {
                    return response;
                }

                // Handle unsuccessful response
                return new List<ProductToAdd>();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Unable to get list of products.","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                Console.WriteLine($"Error: {ex.Message}");
                return new List<ProductToAdd>();
            }
        }
        public async Task<List<CustomerReport>> GetListOfCustomers()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CustomerReport>>("api/customer/get-customers");

                if (response != null) { return response; }

                // Handle unsuccessful response
                return new List<CustomerReport>();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Unable to get list of customers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Error: {ex.Message}");
                return new List<CustomerReport>();
            }
        }
        public async Task<int> UpdateCustomer(CustomerReport customer)
        {
            try
            {
                if (customer == null)
                {
                    return -901;
                }

                //Convert customer to JSON
                var jsonCustomer = JsonConvert.SerializeObject(customer);

                //Create string content from JSON
                var content = new StringContent(jsonCustomer, Encoding.UTF8, "application/json");

                //Send HTTP PUT reguest to API endpoint
                var response = await _httpClient.PutAsync("api/customer/update-customer", content);

                //Check if response was successfull
                if (response.IsSuccessStatusCode)
                {
                    return 1;
                }

                // If the status code is not 200 OK, handle the error
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update customer. Status Code: {response.StatusCode}, Error: {errorMessage}");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Unable to update customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Error: {ex.Message}");
                return -900;
            }
        }
        public async Task<int> AddCustomer(CustomerReport customer)
        {
            try
            {
                if (customer == null)
                {
                    return -901;
                }

                //Convert customer to JSON
                var jsonCustomer = JsonConvert.SerializeObject(customer);

                //Create string content from JSON
                var content = new StringContent(jsonCustomer, Encoding.UTF8, "application/json");

                //Send HTTP PUT reguest to API endpoint
                var response = await _httpClient.PutAsync("api/customer/create-customer", content);

                //Check if response was successfull
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content to retrieve the customer ID
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (int.TryParse(responseContent, out int custId))
                    {
                        return custId;
                    }
                    else
                    {
                        throw new Exception("Failed to parse order ID from the API response.");
                    }
                }

                // If the status code is not 200 OK, handle the error
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create new customer. Status Code: {response.StatusCode}, Error: {errorMessage}");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Unable to update customer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Error: {ex.Message}");
                return -900;
            }
        }
        public async Task<OrderEditReport> GetOrderDetailAsync(OrderEditReport orderQuery)
        {
            try
            {
                // Convert orderQuery to query string
                var queryString = orderQuery.ToQueryString();

                // Build the full URI with query string
                var uri = $"api/order/get-order-details{queryString}";

                // Send GET request to the order controller
                var response = await _httpClient.GetFromJsonAsync<OrderEditReport>(uri);

                if (response != null)
                {
                    return response;
                }

                return new OrderEditReport();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new OrderEditReport();
            }
        }
        public async Task<bool> UpdateOrderAsync(OrderEditReport orderQuery)
        {
            try
            {
                // Convert OrderEditReport to JSON
                var jsonOrder = JsonConvert.SerializeObject(orderQuery);

                // Create StringContent from JSON
                var content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

                // Send HTTP PUT request to the API endpoint
                var response = await _httpClient.PutAsync("api/order/update-order", content);

                // Check if the request was successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // If the status code is not 200 OK, handle the error
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update order. Status Code: {response.StatusCode}, Error: {errorMessage}");
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                throw new Exception($"Failed to update order. {ex.Message}");
            }
        }
        public async Task<int> CreateNewOrder(OrderEditReport orderQuery)
        {
            try
            {
                // Convert OrderEditReport to JSON
                var jsonOrder = JsonConvert.SerializeObject(orderQuery);

                // Create StringContent from JSON
                var content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

                // Send HTTP PUT request to the API endpoint
                var response = await _httpClient.PutAsync("api/order/create-order", content);

                // Check if the request was successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response content to retrieve the order ID
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (int.TryParse(responseContent, out int orderId))
                    {
                        return orderId;
                    }
                    else
                    {
                        throw new Exception("Failed to parse order ID from the API response.");
                    }
                }
                else
                {
                    throw new Exception($"Failed to create order. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                throw new Exception($"Failed to update order. {ex.Message}");
            }
        }
        public async Task<bool> DeleteProductFromOrder(int orderId, int productId)
        {
            try
            {
                var queryString = $"?orderId={orderId}&productId={productId}";

                // Build the full URI with query string
                var uri = $"api/order/delete-product{queryString}";

                var response = await _httpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                throw new Exception($"Failed to delete a product from order. {ex.Message}");
            }
        }
        public async Task<bool> DeleteOrder(OrderEditReport order)
        {
            try
            {
                //Convert order to query string
                var queryString = order.ToQueryString();

                // Build the full URI with query string
                var uri = $"api/order/delete-order{queryString}";

                // Send DELETE request
                var response = await _httpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;

            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exception
                throw new Exception($"Failed to delete the order. {ex.Message}");
            }
        }
    }
}
