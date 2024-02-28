using System;
using System.Collections.Generic;
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

        public RocketDate(int dayIndex, int dateDay, int dateMonth, int dateYear) 
        {
            DayIndex = dayIndex;
            DateDay = dateDay;
            DateMonth = dateMonth;
            DateYear = dateYear;
        }
    }
}
