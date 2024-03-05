using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public class RocketDate
    {
        public int DayIndex { get; set; }

        public int DateDay { get; set; }

        public int DateMonth { get; set; }

        public int DateYear { get; set; }  

        public bool? IsPlaceholder { get; set; }

        public ObservableCollection<RocketEvent> Events { get; set; }

        public RocketDate(int dayIndex, int dateDay, int dateMonth, int dateYear) 
        {
            DayIndex = dayIndex;
            DateDay = dateDay;
            DateMonth = dateMonth;
            DateYear = dateYear;
            Events = new ObservableCollection<RocketEvent>();
        }

        public RocketDate(bool isPlaceholder)
        {
            if(isPlaceholder)
            {
                DayIndex = 0;
                DateDay = 0;
                DateMonth = 0;
                DateYear = 0;
                Events = new ObservableCollection<RocketEvent>();
            }
        }

        public RocketDate(int dateDay, int dateMonth, int dateYear)
        {
            DateDay = dateDay;
            DateMonth = dateMonth;
            DateYear = dateYear;
            Events = new ObservableCollection<RocketEvent>();
        }

        public RocketDate(int dateDay, int dateMonth, int dateYear, ObservableCollection<RocketEvent> eventsList)
        {
            DateDay = dateDay;
            DateMonth = dateMonth;
            DateYear = dateYear;
            Events = eventsList;
        }
    }
}
