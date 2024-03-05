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
    internal class ListViewHeightConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                if(value != null)
                {
                    return (double)(value) - 20;
                }
                return 100;
            }
            //double width = (double)value;
            Grid grid = value as Grid;
            double height = grid.Height;


            Label label = parameter as Label;
            double parameterText = System.Convert.ToDouble(label.Content);

            return (height / parameterText);
            /*
            ListView listview = value as ListView;
            //double width = listview.Width;

            GridView gv = listview.View as GridView;
            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (!Double.IsNaN(gv.Columns[i].Width))
                    width -= gv.Columns[i].Width;
            }
            return width - 5;// this is to take care of margin/padding
            */
            /*
            if (value == null || parameter == null)
            {
                return 100;
            }

            double height = (double)value;
            Label label = parameter as Label;
            double parameterText = System.Convert.ToDouble(label.Content);

            height = height / parameterText;
            return height;
            */
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var actualGridHeight = (double)values[0];
            var actualRowHeight = actualGridHeight - 20;

            Label label = parameter as Label;
            double parameterText = System.Convert.ToDouble(label.Content);

            /*
            int weekCount = 0;
            IEnumerable enumerable = values[1] as IEnumerable;
            if (enumerable != null)
            {

                foreach (object element in enumerable)
                {
                    weekCount++;
                }
            }
            */

            return (double)(actualRowHeight / parameterText);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
