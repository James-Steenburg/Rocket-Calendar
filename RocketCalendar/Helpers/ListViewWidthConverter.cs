using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RocketCalendar.Helpers
{
    internal class ListViewWidthConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return 100;
            }
            double width = (double)value;

            Label label = parameter as Label;
            double parameterText = System.Convert.ToDouble(label.Content);

            return (width / parameterText);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var actualWidth = (double)values[0];

            Label label = parameter as Label;
            double parameterText = System.Convert.ToDouble(label.Content);
            return (double)(actualWidth / parameterText);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
