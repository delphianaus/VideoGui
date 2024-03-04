using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.ffmpeg.Events
{
    public delegate void ConverterProbDataEventHandler(object sender, int ProbeID, string args);
    public delegate void ConverterOnDataEventHandler(object sender, string data,string filename,int processid);
    public delegate void ConverterOnProgressEventHandler(object sender, string filename,int percent ,TimeSpan Duration,TimeSpan totallength, int processid);
    public delegate void ConverterOnSeekEventHandler(object sender, string filename,string seekinfo, int processid);
    public delegate void ConverterOnStartedEventHandler(object sender, string filename,int processid);
    public delegate void ConverterOnStoppedEventHandler(object sender, string filename, int processid, int exitcode, List<string> ErrorDetails);
}
