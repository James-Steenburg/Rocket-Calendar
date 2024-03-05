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

            ListView listview = value as ListView;
            //double width = listview.Width;

            GridView gv = listview.View as GridView;
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (!Double.IsNaN(gv.Columns[i].Width))
                    width -= gv.Columns[i].Width;
            }
            return width - 5;// this is to take care of margin/padding

            /*
            if(value == null || parameter == null)
            {
                return 100;
            }
            double width = (double)value;
            Label label = parameter as Label;
            double parameterText = System.Convert.ToDouble(label.Content); 

            width = width / parameterText;
            //width -= 5;
            return width;
            */
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
            /*

            int dayCount = 0;
            IEnumerable enumerable = values[1] as IEnumerable;
            if (enumerable != null)
            {

                foreach (object element in enumerable)
                {
                    dayCount++;
                }
            }
            */
            return (double)(actualWidth / parameterText);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
