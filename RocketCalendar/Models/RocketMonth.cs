using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Models
{
    public class RocketMonth
    {
        public string Name { get; set; }

        public int NumOfDays { get; set; }

        public RocketMonth(string name, int numOfDays)
        {
            Name = name;
            NumOfDays = numOfDays;
        }
    }
}
