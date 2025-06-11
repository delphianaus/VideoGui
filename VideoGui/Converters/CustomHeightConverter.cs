using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VideoGui
{ 
    public class CustomHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return 0;

            // Get ActualHeight (double) and Tag (should be convertible to double)
            double height = 0;
            double subtract = 0;

            if (values[0] is double h)
                height = h;

            // Tag can be int, double, or string
            if (values[1] is double d)
                subtract = d;
            else if (values[1] is int i)
                subtract = i;
            else if (values[1] != null && double.TryParse(values[1].ToString(), out double parsed))
                subtract = parsed;

            double result = height - subtract;
            return result > 0 ? result : 0;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
