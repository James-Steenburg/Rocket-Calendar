using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public partial class GlobalAppData : ObservableObject
    {
        [ObservableProperty]
        private RocketCalendarModel _activeRocketCalendar;

        [ObservableProperty]
        private bool _IS_DEBUG_MODE = true;
        
    }
}
