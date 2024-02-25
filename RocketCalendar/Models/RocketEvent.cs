using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public class RocketEvent
    {
        public RocketDate EventDate { get; set; }
        
        public string EventName { get; set; }

        public string EventDescription { get; set; }

        public bool IsPrivate { get; set; }

        public RocketEvent(RocketDate eventDate, string eventName, string eventDescription, bool isPrivate)
        {
            EventDate = eventDate;
            EventName = eventName;
            EventDescription = eventDescription;
            IsPrivate = isPrivate;
        }
    }
}
