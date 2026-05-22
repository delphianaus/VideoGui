using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    class AdobeClipData
    {
        public int clipnumber = 0;
        public int start = 0;
        public int end = 0;

        public AdobeClipData(int _cn, int _start , int _end)
        {
            clipnumber = _cn;
            start = _start;
            end = _end;
        }
    }
}
