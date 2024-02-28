using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RocketCalendar.Models
{
    public class RocketBrush
    {
        public System.Windows.Media.Brush BrushColor { get; set; }

        public string BrushName { get; set; }

        public RocketBrush(Brush brush, string brushName) 
        {
            BrushColor = brush; 
            BrushName = brushName;
        }
    }
}
