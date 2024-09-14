using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg
{
    internal class FilterConfiguration : IFilterConfiguration
    {
        /// <inheritdoc />
        public string FilterType { get; set; }

        /// <inheritdoc />
        public int StreamNumber { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
