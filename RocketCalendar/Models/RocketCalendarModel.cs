using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public partial class RocketCalendarModel : ObservableObject
    {
        [ObservableProperty]
        private string _calendarName;

        [ObservableProperty]
        private RocketDate _baseDate;

        [ObservableProperty]
        private ObservableCollection<RocketMonth> _monthCollection;

        [ObservableProperty]
        private ObservableCollection<string> _dayNameCollection;

        [ObservableProperty]
        private ObservableCollection<RocketEvent> _eventCollection;

        [ObservableProperty]
        private int _currentMonth;

        [ObservableProperty]
        private int _currentYear;

        [ObservableProperty]
        private RocketEvent _wipEventPlaceholder;

        //[ObservableProperty]
        //private ObservableCollection<RocketEvent> _repeatingEventCollection;

        public RocketCalendarModel
            (
            string calendarName,
            RocketDate baseDate, 
            ObservableCollection<RocketMonth> monthCollection, 
            ObservableCollection<string> dayNameCollection, 
            ObservableCollection<RocketEvent> eventCollection, 
            int currentMonth, 
            int currentYear
            )
        {
            CalendarName = calendarName;
            BaseDate = baseDate;
            MonthCollection = monthCollection;
            DayNameCollection = dayNameCollection;
            EventCollection = eventCollection;
            CurrentMonth = currentMonth;
            CurrentYear = currentYear;
        }
    }
}
