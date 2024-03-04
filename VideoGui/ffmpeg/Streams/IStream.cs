using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg.Streams
{
    public interface IStream
    {
        /// <summary>
        ///     File source of stream
        /// </summary>
        string Path { get; set; }

        bool PreInput { get; }
        bool PostInput { get; }

        /// <summary>
        ///     Index of stream
        /// </summary>
        int Index { get; }


        bool IsCopy { get; }

        /// <summary>
        ///     Format
        /// </summary>
        string Codec { get; }

        /// <summary>
        ///     Build FFmpeg arguments for input
        /// </summary>
        /// <returns>Arguments</returns>
        string BuildParameters(bool forPosition);

        /// <summary>
        ///     Get stream input
        /// </summary>
        /// <returns>Input path</returns>
        IEnumerable<string> GetSource();

        /// <summary>
        ///     Codec type
        /// </summary>
        StreamType StreamType { get; }
    }
}
