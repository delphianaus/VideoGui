using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg
{
    public interface IFilterConfiguration
    {
        /// <summary>
        ///     Type of filter
        /// </summary>
        string FilterType { get; }

        /// <summary>
        ///     Stream filter number
        /// </summary>
        int StreamNumber { get; }

        /// <summary>
        ///     Filter with name and values
        /// </summary>
        Dictionary<string, string> Filters { get; }
    }
}
