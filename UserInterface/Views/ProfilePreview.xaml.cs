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

namespace UserInterface.Views
{
    /// <summary>
    /// Interaction logic for ProfilePreview.xaml
    /// </summary>
    public partial class ProfilePreview : UserControl
    {
        public ProfilePreview()
        {
            InitializeComponent();

            //Get user photo
            Utils utils = new Utils();
            string? directoryPath = Application.Current.FindResource("PhotoDirectory") as string;
            utils.PhotoUrl = utils.UpdatePhotoUrl(CurrentUser.PhotoFileName, directoryPath);
            userPhoto.DataContext = utils;

            //Populate current user data
            txtName.Text = CurrentUser.FirstName + " " + CurrentUser.LastName;
            txtPosition.Text = CurrentUser.Position;

        }

        private void btnViewProfile_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).OnViewProfileProfilePreview();
        }

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword changePassword = new();
            changePassword.ShowDialog();
        }
    }
}
