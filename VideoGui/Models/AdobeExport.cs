using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VideoGui.Models
{
    public class AdobeExport
    {
        public string filename = "";
        public int In = 0, Out = 0, Start = 0, End = 0, Duration = 0;
        public double Offset = 0.0;
        public bool IsCutPoint = false, Delete = false;
        public bool IsLast = false;
        public AdobeExport(string _filename,int _In,int _Out,int _Start,int _End,int _dur)
        {
            filename = _filename;
            In = _In;
            Out = _Out;
            Start = _Start;
            End = _End;
            Duration = _dur;
            IsCutPoint = (Duration != (Out - In));
        }

        public int Used => Out - In;
        public bool IsStartGap => (IsCutPoint) ? In > 0 : false;
        public bool IsEndGap => (IsCutPoint) ? Out < Duration : false;

        public bool IsGap => (IsEndGap || IsStartGap);
    }
}
