using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserInterface.Services
{
    public static class CurrentUser
    {
        public static JwtSecurityToken? Token {  get; set; }
        public static int EmployeeId { get; set; } = 0;
        public static string Title { get; set; } = string.Empty;
        public static string FirstName { get; set; } = string.Empty;
        public static string LastName { get; set; } = string.Empty;
        public static string Position { get; set; } = string.Empty;
        public static string BirthDate { get; set; } = string.Empty;
        public static string HireDate { get; set; } = string.Empty;
        public static string Address { get; set; } = string.Empty;
        public static string City { get; set; } = string.Empty;
        public static string PostalCode { get; set; } = string.Empty;
        public static string Country { get; set; } = string.Empty;
        public static string Phone { get; set; } = string.Empty;
        public static int Role { get; set; } = 0;
        public static string Username { get; set; } = string.Empty;
        public static string Manager { get; set; } = string.Empty;
        public static string PhotoFileName { get; set; } = string.Empty;
        public static bool PassNotChanged { get; set; } = false;

        public enum Roles : int
        {
            Admin,
            Sales,
            SalesMngr,
            Logistics,
            LogisticsMngr,
            Production,
            ProductionMngr,
            Hr,
            Unasigned
        }


        #region User photo
        //private static string _photoFileName = string.Empty;
        //public string PhotoFileName
        //{
        //    get => _photoFileName;
        //    set
        //    {
        //        if (!string.IsNullOrEmpty(value))
        //        {
        //            _photoFileName = value;
        //            UpdatePhotoUrl();
        //            OnPropertyChanged(nameof(PhotoFileName));
        //        }
        //    }
        //}

        //private static string _photoUrl = string.Empty;
        //public string PhotoUrl
        //{
        //    get => _photoUrl;
        //    private set
        //    {
        //        if (_photoUrl != value)
        //        {
        //            _photoUrl = value;
        //            OnPropertyChanged(nameof(PhotoUrl));
        //        }
        //    }
        //}

        //private void UpdatePhotoUrl()
        //{
        //    // Access the configuration value from App.xaml
        //    string directoryPath = Application.Current.FindResource("PhotoDirectory") as string;

        //    if (string.IsNullOrEmpty(directoryPath))
        //    {
        //        // Default path if configuration is missing or empty

        //        // Create the directory if it doesn't exist
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath);
        //        }
        //    }

        //    PhotoUrl = Path.Combine(directoryPath, PhotoFileName);
        //}

        #endregion

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


    }
}
