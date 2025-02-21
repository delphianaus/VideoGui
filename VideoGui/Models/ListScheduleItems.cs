using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class ListScheduleItems
    {
        private int _gap = -1;
        private TimeOnly _Start = TimeOnly.FromTimeSpan(TimeSpan.Zero), _End = TimeOnly.FromTimeSpan(TimeSpan.Zero);
        public int Gap { get { return _gap; } set { _gap = value; } }
        public TimeOnly Start { get { return _Start; } set { _Start = value; } }
        public TimeOnly End { get { return _End; } set { _End = value; } }
        public ListScheduleItems(TimeOnly start, TimeOnly end, int gap)
        {
            Gap = gap;
            Start = start;
            End = end;
        }

        public ListScheduleItems(TimeSpan start, TimeSpan end, int gap)
        {
            Gap = gap;
            Start = TimeOnly.FromTimeSpan(start);
            End = TimeOnly.FromTimeSpan(end); //end;
        }
    }
}
