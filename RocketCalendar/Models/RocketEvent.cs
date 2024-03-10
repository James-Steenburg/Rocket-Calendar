using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public partial class RocketEvent : ObservableObject
    {
        [ObservableProperty]
        private RocketDate _eventDate;

        [ObservableProperty]
        private string _eventName;

        [ObservableProperty]
        private string _eventDescription;

        [ObservableProperty]
        private bool _isPrivate;

        [ObservableProperty]
        private int _colorIndex;

        
        public bool IsRepeatingEvent
        {
            get
            {
                if (WeekRepeatInterval == 0 && MonthRepeatInterval == 0 && YearRepeatInterval == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRepeatingEvent))]
        private int _weekRepeatInterval;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRepeatingEvent))]
        private int _monthRepeatInterval;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRepeatingEvent))]
        private int _yearRepeatInterval;

        public RocketEvent(RocketDate eventDate, string eventName, string eventDescription, bool isPrivate, int colorIndex)
        {
            EventDate = eventDate;
            EventName = eventName;
            EventDescription = eventDescription;
            IsPrivate = isPrivate;
            ColorIndex = colorIndex;
            WeekRepeatInterval = 0;
            MonthRepeatInterval = 0;
            YearRepeatInterval = 0;
        }
        public RocketEvent(RocketDate eventDate, string eventName, string eventDescription, bool isPrivate, int colorIndex, int weekRepeatInterval, int monthRepeatInterval, int yearRepeatInterval)
        {
            EventDate = eventDate;
            EventName = eventName;
            EventDescription = eventDescription;
            IsPrivate = isPrivate;
            ColorIndex = colorIndex;
            WeekRepeatInterval = weekRepeatInterval;
            MonthRepeatInterval = monthRepeatInterval;
            YearRepeatInterval = yearRepeatInterval;
        }

    }
}
