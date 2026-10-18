using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Text;

namespace VideoGui.Models
{
    public class ffmpegProcesses
    {
        public List<ffmpegProcess> Processs = [];

    }

    public class ffmpegProcess
    {
        public int id { get; set; }
        public string command { get; set; }
        public Nullable<DateTime> FirstSeen { get; set; } = null;
        public Nullable<DateTime> LastSeen { get; set; } = null;

        public ffmpegProcess(int _id, string _cmd)
        {
            id = _id;
            command = _cmd;
            FirstSeen = DateTime.Now;
        }

    }





}
