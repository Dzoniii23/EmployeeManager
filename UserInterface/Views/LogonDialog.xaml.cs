using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
using UserInterface.Models;
using UserInterface.Services;
using static UserInterface.Services.CurrentUser;
using SharedLibrary;

namespace UserInterface.Views
{
    /// <summary>
    /// Interaction logic for LogonDialog.xaml
    /// </summary>
    public partial class LogonDialog : UserControl
    {
        #region Event Members
        public event EventHandler LogonSuccess;
        public event EventHandler LogonFailed;
        #endregion

        #region Constructor
        public LogonDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Events
        private async void btnLogin_ClickAsync(object sender, RoutedEventArgs e)
        {
            //await LogonAsync();
            // Show loading indicator and disable UI
            loadingPanel.Visibility = Visibility.Visible;

            try
            {
                // Call your login method
                await LogonAsync();

                // If login is successful, hide loading indicator
                loadingPanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                // Handle login failure
                DisplayError($"Error during login: {ex.Message}");
            }
            finally
            {
                // Ensure loading indicator is hidden even in case of an exception
                loadingPanel.Visibility = Visibility.Collapsed;
            }
        }

        private async void UserControl_KeyDownAsync(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                //await LogonAsync();
                // Show loading indicator and disable UI
                loadingPanel.Visibility = Visibility.Visible;

                try
                {
                    // Call your login method
                    await LogonAsync();

                    // If login is successful, hide loading indicator
                    loadingPanel.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    // Handle login failure
                    DisplayError($"Error during login: {ex.Message}");
                }
                finally
                {
                    // Ensure loading indicator is hidden even in case of an exception
                    loadingPanel.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region Authentication
        private async Task LogonAsync()
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Password))
            {
                // Create an instance of the ApiService
                var apiService = new ApiService();

                // Create a LoginModel object with the entered credentials
                var loginModel = new LoginModel
                {
                    UserName = txtUsername.Text,
                    Password = txtPassword.Password
                };

                try
                {
                    // Call the API service to attempt login
                    var responseString = await apiService.LoginAsync(loginModel);

                    // Deserialize the entire response string into AuthResponse class
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseString);

                    if (!string.IsNullOrEmpty(authResponse.Token))
                    {
                        // Access EmployeeReport from the deserialized response
                        var employeeReport = authResponse.EmployeeReport;

                        // Get the JwtSecurityToken from the handler
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadToken(authResponse.Token) as JwtSecurityToken;

                        // Call the PopulateCurrentUserData method to update CurrentUser
                        PopulateCurrentUserData(jwtToken, employeeReport);

                        // Raise the LogonSuccess event
                        Utils.NavigateTo = CurrentUser.Role;
                        LogonSuccess?.Invoke(this, new EventArgs());
                    }
                    else
                    {
                        // Handle the case where Token is null or empty
                        DisplayError("Invalid credentials.");
                        LogonFailed?.Invoke(this, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network errors, server issues)
                    if (ex != null)
                    {
                        // Log the full exception details for debugging
                        Console.WriteLine($"Exception details: {ex}");

                        // Display a user-friendly error message
                        DisplayError($"Error during login: {ex.Message}");
                    }
                    else
                    {
                        // Log that the exception object is null
                        Console.WriteLine("Exception object is null");
                        DisplayError("An error occurred during login.");
                    }

                    LogonFailed?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                // Display error for empty fields
                DisplayError("Both fields must be filled.");
                LogonFailed?.Invoke(this, new EventArgs());
            }
        }
        #endregion

        #region Helper Methods
        private void DisplayError(string message)
        {
            if (!string.IsNullOrEmpty(message)) {
                txtError.Text = message;
                txtError.Visibility = Visibility.Visible;
            }
            else { 
                txtError.Visibility = Visibility.Collapsed;
                txtError.Text = string.Empty;
            }
        }
        private void PopulateCurrentUserData(JwtSecurityToken token, EmployeeReport employeeReport)
        {
            if (token != null)
            {
                // Extract user data from the EmployeeReport object
                CurrentUser.Token = token;
                CurrentUser.EmployeeId = employeeReport.EmpId;//.ToString();
                CurrentUser.Title = employeeReport.TitleOfCourtesy;
                CurrentUser.FirstName = employeeReport.FirstName;
                CurrentUser.LastName = employeeReport.LastName;
                CurrentUser.Position = employeeReport.Title;
                CurrentUser.BirthDate = employeeReport.BirthDate.ToString("D");
                CurrentUser.HireDate = employeeReport.HireDate.ToString("D");
                CurrentUser.Address = employeeReport.Address;
                CurrentUser.City = employeeReport.City;
                CurrentUser.PostalCode = employeeReport.PostalCode;
                CurrentUser.Country = employeeReport.Country;
                CurrentUser.Phone = employeeReport.Phone;
                CurrentUser.PassNotChanged = employeeReport.PassNotChanged;

                // Convert the role from string to enum
                if (Enum.TryParse(typeof(Roles), employeeReport.Role, out var roleEnum))
                {
                    CurrentUser.Role = (int)roleEnum;
                }
                else
                {
                    // Handle the case where the role string doesn't match any enum value
                    CurrentUser.Role = (int)CurrentUser.Roles.Unasigned; // Default role
                }

                CurrentUser.Username = employeeReport.Username;
                CurrentUser.Manager = employeeReport.Manager;
                CurrentUser.PhotoFileName = employeeReport.PhotoName;

                // Display a success message (this is only for testing)
                DisplayError("Logon success.");
            }
            else
            {
                DisplayError("Error parsing JWT token.");
            }
        }
        #endregion
    }
}
