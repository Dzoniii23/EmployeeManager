using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using UserInterface.Views;
using UserInterface.Windows;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();

            BtnBackUserProfileClicked += HandleOnBackUserProfile;
            BtnViewProfileProfilePreviewClicked += HandleOnViewProfileProfilePreview;

            Utils.NavigateTo = 100; //Go to logon

            Refresh();
        }
        #endregion

        #region Events
        //---------------- Catch LogonSuccess from LogonDialog.xaml
        private async void LogonDialog_LogonSuccess(object sender, EventArgs e)
        {
            await Task.Delay(1000);
            LogonDialog.Visibility = Visibility.Collapsed;

            if (CurrentUser.PassNotChanged)
            {
                bool changePasswordSuccessful = false;
                int maxAttempts = 3;
                int attempts = 0;

                while (!changePasswordSuccessful && attempts < maxAttempts)
                {
                    ChangePassword changePassword = new();
                    changePassword.ShowDialog();

                    if (changePassword.IsSuccess)
                    {
                        changePasswordSuccessful = true;
                    }
                    else
                    {
                        MessageBoxResult result =  MessageBox.Show("Password change was not successful. Please try again.",
                            "Unsuccessful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK);

                        if (result == MessageBoxResult.OK)
                        {
                            // Increment the attempts counter
                            attempts++;
                        }                        
                    }
                }

                if (changePasswordSuccessful)
                {
                    Refresh();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("We've noticed multiple unsuccessful attempts to change your password. For security reasons, we're unable to process further attempts. If you're having trouble, please contact our support team for assistance. Goodbye.",
                            "Unsuccessful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation,
                            MessageBoxResult.OK);

                    if (result == MessageBoxResult.OK)
                    {
                        LogOff logOff = new();
                        logOff.PerformLogOff();
                        Close();
                    }
                }
            }
            else
            {
                Refresh();
            }
        }
        //----------------

        //---------------- Catch back button from UserProfile.xaml

        //Delegate Definition
        public delegate void BtnBackUserProfileEventHandler(object sender, EventArgs e);

        //Event Declaration
        public event BtnBackUserProfileEventHandler BtnBackUserProfileClicked;

        //Event Trigger Method
        internal void OnBackUserProfile()
        {
            BtnBackUserProfileClicked?.Invoke(this, EventArgs.Empty);
        }

        //Event Handler Method
        private void HandleOnBackUserProfile(object sender, EventArgs e)
        {
            if (CurrentUser.Role >= (int)CurrentUser.Roles.Admin && 
                CurrentUser.Role <= (int)CurrentUser.Roles.Hr )
            {
                Utils.NavigateTo = CurrentUser.Role;
                Refresh();
            }
            else
            {
                throw new Exception("User role is not valid");
            }            
        }
        //----------------

        //---------------- Catch view profile button from ProfilePreview.xaml
        public delegate void BtnViewProfileProfilePreviewEventHandler(object sender, EventArgs e);
        public event BtnBackUserProfileEventHandler BtnViewProfileProfilePreviewClicked;
        internal void OnViewProfileProfilePreview()
        {
            BtnViewProfileProfilePreviewClicked?.Invoke(this, EventArgs.Empty);
        }
        private void HandleOnViewProfileProfilePreview(object sender, EventArgs e)
        {
            Utils.NavigateTo = 101;
            Refresh();
        }
        //----------------
        #endregion

        #region Refresh
        public void Refresh()
        {
            try {
                switch (Utils.NavigateTo)
                {
                    //TODO: Finish navigation to all relevant pages
                    case 100: //Go to Logon page
                        LogonDialog.Visibility = Visibility.Visible;
                        break;

                    case 101: //Go to User profile                        
                        mainFrame.Navigate(new UserProfile());
                        break;

                    case (int)CurrentUser.Roles.Admin:
                        mainFrame.Navigate (new ViewAdministrator());
                        break;

                    case (int)CurrentUser.Roles.Sales:
                        mainFrame.Navigate(new ViewSales());
                        break;

                    case (int)CurrentUser.Roles.SalesMngr:
                        mainFrame.Navigate(new ViewSales());
                        break;
                }
            } 
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error fetching details", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
