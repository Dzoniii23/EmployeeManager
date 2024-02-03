using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UserInterface.Services
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return $"{decimalValue:P2}"; // P2 format specifier for percentage with 2 decimal places
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Convert string to decimal
                if (decimal.TryParse(stringValue, out decimal result))
                {
                    return result / 100; // Divide by 100 to convert from percentage to decimal
                }
            }

            return value;
        }
    }
}
