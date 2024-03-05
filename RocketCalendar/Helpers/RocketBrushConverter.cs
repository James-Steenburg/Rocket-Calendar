using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RocketCalendar.Helpers
{
    internal class RocketBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((int)value >= 0 &&  (int)value < RocketBrushList.Count)
            {
                return RocketBrushList[(int)value].BrushColor;
            }
            else
            {
                return System.Windows.Media.Brushes.Red;
            }
        }

        private ObservableCollection<RocketBrush> RocketBrushList = new ObservableCollection<RocketBrush>()
        {
            new RocketBrush(System.Windows.Media.Brushes.Transparent, "Transparent"),
            new RocketBrush(System.Windows.Media.Brushes.DarkGray, "Dark Gray"),
            new RocketBrush(System.Windows.Media.Brushes.Firebrick, "Firebrick"),
            new RocketBrush(System.Windows.Media.Brushes.Gold, "Gold"),
            new RocketBrush(System.Windows.Media.Brushes.LightGoldenrodYellow, "Light Yellow Goldenrod"),
            new RocketBrush(System.Windows.Media.Brushes.LightGray, "Light Gray"),
            new RocketBrush(System.Windows.Media.Brushes.LightSkyBlue, "Light Sky Blue"),
            new RocketBrush(System.Windows.Media.Brushes.MediumAquamarine, "Medium Aquamarine"),
            new RocketBrush(System.Windows.Media.Brushes.MediumBlue, "Medium Blue"),
            new RocketBrush(System.Windows.Media.Brushes.MediumOrchid, "Medium Orchid"),
            new RocketBrush(System.Windows.Media.Brushes.MediumPurple, "Medium Purple"),
            new RocketBrush(System.Windows.Media.Brushes.MediumSeaGreen, "Medium Sea Green"),
            new RocketBrush(System.Windows.Media.Brushes.MediumSlateBlue, "Medium Slate Blue"),
            new RocketBrush(System.Windows.Media.Brushes.MediumSpringGreen, "Medium Spring Green"),
            new RocketBrush(System.Windows.Media.Brushes.MediumTurquoise, "Medium Turquoise"),
            new RocketBrush(System.Windows.Media.Brushes.MediumVioletRed, "Medium Violet Red"),
            new RocketBrush(System.Windows.Media.Brushes.Pink, "Pink"),
            new RocketBrush(System.Windows.Media.Brushes.Salmon, "Salmon"),
            new RocketBrush(System.Windows.Media.Brushes.SeaGreen, "Sea Green"),
            new RocketBrush(System.Windows.Media.Brushes.Tomato, "Tomato")
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
