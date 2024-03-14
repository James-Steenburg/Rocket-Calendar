using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RocketCalendar.Models
{
    public partial class GlobalAppData : ObservableObject
    {
        [ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(CalendarNavigationVisibility))]
        private RocketCalendarModel _activeRocketCalendar;

        [ObservableProperty]
        private bool _IS_DEBUG_MODE = false;


        /*
         
         public Visibility CalendarNavigationVisibility
        {
            get
            {
                if (_activeRocketCalendar == null)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }
         */



    }
}
