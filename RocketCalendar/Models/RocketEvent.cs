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

        public RocketEvent(RocketDate eventDate, string eventName, string eventDescription, bool isPrivate, int colorIndex)
        {
            EventDate = eventDate;
            EventName = eventName;
            EventDescription = eventDescription;
            IsPrivate = isPrivate;
            ColorIndex = colorIndex;
        }
    }
}
