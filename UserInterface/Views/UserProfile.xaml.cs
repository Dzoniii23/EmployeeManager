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
using UserInterface.Windows;

namespace UserInterface.Views
{
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : UserControl
    {
        public UserProfile()
        {
            InitializeComponent();

            
            
            Refresh();
        }

        #region Events

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).OnBackUserProfile();
        }        

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changePassword = new();
            changePassword.ShowDialog();

            Refresh();
        }

        #endregion

        private void Refresh()
        {
            //BitmapImage tempImg = new BitmapImage();
            //tempImg.BeginInit();
            //tempImg.UriSource = new Uri(CurrentUser.PhotoUrl);
            //tempImg.EndInit();
            //userPhoto.Source = tempImg;

            //Get user photo
            Utils utils = new Utils();
            string? directoryPath = Application.Current.FindResource("PhotoDirectory") as string;
            utils.PhotoUrl = utils.UpdatePhotoUrl(CurrentUser.PhotoFileName, directoryPath);
            userPhoto.DataContext = utils;

            //Populate current user data
            txtLn1.Text = CurrentUser.EmployeeId.ToString();
            txtLn2.Text = CurrentUser.Title.ToString();
            txtLn3.Text = CurrentUser.FirstName.ToString();
            txtLn4.Text = CurrentUser.LastName.ToString();
            txtLn5.Text = CurrentUser.Position.ToString();
            txtLn6.Text = CurrentUser.BirthDate.ToString();
            txtLn7.Text = CurrentUser.HireDate.ToString();
            txtLn8.Text = CurrentUser.Address.ToString();
            txtLn9.Text = CurrentUser.City.ToString();
            txtLn10.Text = CurrentUser.PostalCode.ToString();
            txtLn11.Text = CurrentUser.Country.ToString();
            txtLn12.Text = CurrentUser.Phone.ToString();
            txtLn13.Text = Enum.GetName(typeof(CurrentUser.Roles), CurrentUser.Role); //CurrentUser.Role.ToString();
            txtLn14.Text = CurrentUser.Username.ToString();
            txtLn15.Text = CurrentUser.Manager.ToString();
        }
    }
}
