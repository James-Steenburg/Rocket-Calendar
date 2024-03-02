using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public class GlobalAppData
    {
        public RocketCalendarModel ActiveRocketCalendar { get; set; }
        
        public bool IS_DEBUG_MODE
        {
            get { return true; }
        }
    }
}
