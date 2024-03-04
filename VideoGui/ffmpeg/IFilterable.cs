using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg
{
    internal interface IFilterable
    {
        IEnumerable<IFilterConfiguration> GetFilters();
    }
}
