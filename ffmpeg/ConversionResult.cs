using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg
{
    internal class ConversionResult : IConversionResult
    {
        /// <inheritdoc />
        public DateTime StartTime { get; internal set; }

        /// <inheritdoc />
        public DateTime EndTime { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration => EndTime - StartTime;

        /// <inheritdoc />
        public string Arguments { get; internal set; }
    }
}
