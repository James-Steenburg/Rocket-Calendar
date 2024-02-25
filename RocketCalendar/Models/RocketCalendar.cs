using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public class RocketCalendar
    {
        public RocketDate BaseDate { get; set; }

        public ObservableCollection<RocketMonth> MonthCollection { get; set; }

        public ObservableCollection<string> DayNameCollection { get; set; }

        public ObservableCollection<RocketEvent> EventCollection { get; set; }

        public int CurrentMonth { get; set; }

        public int CurrentYear { get; set; }

        public RocketCalendar
            (
            RocketDate baseDate, 
            ObservableCollection<RocketMonth> monthCollection, 
            ObservableCollection<string> dayNameCollection, 
            ObservableCollection<RocketEvent> eventCollection, 
            int currentMonth, 
            int currentYear
            )
        {
            BaseDate = baseDate;
            MonthCollection = monthCollection;
            DayNameCollection = dayNameCollection;
            EventCollection = eventCollection;
            CurrentMonth = currentMonth;
            CurrentYear = currentYear;
        }
    }
}
