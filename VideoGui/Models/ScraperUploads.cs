using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class ScraperUploads
    {
        public string file = "";
        public bool uploaded = false, finished = false;
        public ScraperUploads(string _file, bool _uploaded, bool _finished)
        {
            file = _file;
            uploaded = _uploaded;
            finished = _finished;
        }

    }
}
