using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public class RocketWeekModel
    {
        public ObservableCollection<RocketDate> RocketWeek {  get; set; }

        public RocketWeekModel(ObservableCollection<RocketDate> week) 
        {
            RocketWeek = week;
        }

        public RocketWeekModel()
        {
            RocketWeek = new ObservableCollection<RocketDate>();
        }
    }
}
