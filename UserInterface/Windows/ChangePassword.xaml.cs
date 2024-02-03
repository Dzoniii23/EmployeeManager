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
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        // Property to store the result
        public bool IsSuccess { get; private set; }

        public ChangePassword()
        {
            InitializeComponent();
        }
        private async void txtNewPasswordRepeat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ExecuteChange();
            }                
        }
        private async void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteChange();
        }
        private async Task ExecuteChange()
        {
            ChangePassModel? changePassModel = new ChangePassModel();
            changePassModel = CheckData();

            if (changePassModel != null)
            {
                await ChangePass(changePassModel);
                return;
            }

            IsSuccess = false;
            return;            
        }
        private ChangePassModel? CheckData()
        {
            if (string.IsNullOrEmpty(txtOldPassword.Password) ||
                string.IsNullOrEmpty(txtNewPassword.Password) ||
                string.IsNullOrEmpty(txtNewPasswordRepeat.Password))
            {
                MessageBoxResult result = MessageBox.Show("All fields must be filled. Please try again.",
                            "Unsuccessful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK);

                if (result == MessageBoxResult.OK)
                {
                    return null;
                }
            }

            if (txtNewPassword.Password != txtNewPasswordRepeat.Password)
            {
                MessageBoxResult result = MessageBox.Show("New password and new password repeat must be the same. Please try again.",
                            "Unsuccessful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK);

                if (result == MessageBoxResult.OK)
                {
                    return null;
                }
            }

            // Check the new password strength
            if (!IsStrongPassword(txtNewPassword.Password))
            {
                MessageBoxResult result = MessageBox.Show("New password does not meet the required strength criteria.\n\n" +
                    "Rules for the new password:\n" +
                    "  1. Minimum length of 8 characters.\n" +
                    "  2. At least one uppercase letter.\n" +
                    "  3. At least one lowercase letter.\n" +
                    "  4. At least one digit or special character.",
                            "Unsuccessful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK);

                if (result == MessageBoxResult.OK)
                {
                    return null;
                }
            }

            var changePassModel = new ChangePassModel
            {
                EmpId = CurrentUser.EmployeeId,
                NewPassword = txtNewPassword.Password,
                OldPassword = txtOldPassword.Password
            };

            return changePassModel;
        }
        private bool IsStrongPassword(string password)
        {
            // Minimum length of 8 characters
            if (password.Length < 8)
            {
                return false;
            }

            // At least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                return false;
            }

            // At least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                return false;
            }

            // At least one digit or special character
            if (!password.Any(char.IsDigit) && !password.Any(char.IsSymbol) && !password.Any(char.IsPunctuation))
            {
                return false;
            }

            return true;
        }

        private async Task ChangePass(ChangePassModel model)
        {
            // Hide old message if any
            txtError.Visibility = Visibility.Hidden;

            // Create an instance of the ApiService
            var apiService = new ApiService();

            try
            {
                // Call the API service to change password
                var responseString = await apiService.ChangePass(model);

                if (responseString != null)
                {
                    if (responseString.Contains("Failed"))
                    {
                        // Password change failed
                        MessageBoxResult resultFailed = MessageBox.Show(
                            String.Format("Password change failed due to: {0}", responseString),
                            "Unsuccessful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK);

                        if (resultFailed == MessageBoxResult.OK)
                        {
                            IsSuccess = false;
                            return;
                        }
                    }

                    // Password changed successfully
                    MessageBoxResult resultSuccess = MessageBox.Show(
                        String.Format("Password changed successfully"),
                        "Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation,
                        MessageBoxResult.OK);

                    if (resultSuccess == MessageBoxResult.OK)
                    {
                        IsSuccess = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Password change failed
                MessageBoxResult result = MessageBox.Show(
                    String.Format("Catched exception: {0}", ex.Message.ToString()),
                    "Unsuccessful",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation,
                    MessageBoxResult.OK);

                if (result == MessageBoxResult.OK)
                {
                    IsSuccess = false;
                    return;
                }
            }
            finally
            {
                Close();
            }
        }        
    }
}