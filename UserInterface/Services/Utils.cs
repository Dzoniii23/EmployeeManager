using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace UserInterface.Services
{
    class Utils
    {
        public string PhotoUrl { get; set; } = string.Empty;
        public string UpdatePhotoUrl(string photoName, string directory)
        {
            if (string.IsNullOrEmpty(photoName))
            {
                throw new ArgumentNullException(nameof(photoName));
            }
            else if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            return Path.Combine(directory, photoName);
            //CurrentUser.PhotoUrl = Path.Combine(directory, photoName);

        }

        public static int NavigateTo { get; set; }
        

        //public void UpdatePhotoUrl()
        //{
        //    // Access the configuration value from App.xaml
        //    string directoryPath = Application.Current.FindResource("PhotoDirectory") as string;

        //    if (string.IsNullOrEmpty(directory))
        //    {
        //        // Default path if configuration is missing or empty

        //        // Create the directory if it doesn't exist
        //        if (!Directory.Exists(directory))
        //        {
        //            Directory.CreateDirectory(directory);
        //        }
        //    }

        //    CurrentUser.PhotoUrl = Path.Combine(directory, photoName);
        //}

    }
}
