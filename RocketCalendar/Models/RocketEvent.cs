using CommunityToolkit.Mvvm.ComponentModel;
using OfficeOpenXml.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public partial class RocketEvent : ObservableObject
    {
        [EpplusTableColumn(Order = 0)]
        [ObservableProperty]
        private string _eventName;

        [EpplusIgnore]
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EventDate_Day))]
        [NotifyPropertyChangedFor(nameof(EventDate_Month))]
        [NotifyPropertyChangedFor(nameof(EventDate_Year))]
        private RocketDate _eventDate;

        [EpplusTableColumn(Order = 1)]
        public int EventDate_Day
        {
            get { return EventDate.DateDay; }
        }

        [EpplusTableColumn(Order = 2)]
        public int EventDate_Month
        {
            get { return EventDate.DateMonth; }
        }

        [EpplusTableColumn(Order = 3)]
        public int EventDate_Year
        {
            get { return EventDate.DateYear; }
        }

        [EpplusTableColumn(Order = 8)]
        [ObservableProperty]
        private string _eventDescription;


        [EpplusTableColumn(Order = 4)]
        [ObservableProperty]
        private bool _isPrivate;

        [EpplusTableColumn(Order = 5)]
        [ObservableProperty]
        private int _colorIndex;

        [EpplusIgnore]
        public bool IsRepeatingEvent
        {
            get
            {
                if (MonthRepeatInterval == 0 && YearRepeatInterval == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [EpplusTableColumn(Order = 6)]
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRepeatingEvent))]
        private int _monthRepeatInterval;
        
        [EpplusTableColumn(Order = 7)]
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
            MonthRepeatInterval = 0;
            YearRepeatInterval = 0;
        }
        public RocketEvent(RocketDate eventDate, string eventName, string eventDescription, bool isPrivate, int colorIndex, int monthRepeatInterval, int yearRepeatInterval)
        {
            EventDate = eventDate;
            EventName = eventName;
            EventDescription = eventDescription;
            IsPrivate = isPrivate;
            ColorIndex = colorIndex;
            MonthRepeatInterval = monthRepeatInterval;
            YearRepeatInterval = yearRepeatInterval;
        }
    }
}
