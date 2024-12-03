using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class VideoIdFileName
    {
        private string _VideoId = "";
        private string _FileName = "";

        public string VideoId { get { return _VideoId; } set { _VideoId = value; } }
        public string FileName { get { return _FileName; } set { _FileName = value; } }

        public VideoIdFileName()
        {
            FileName = "";
            VideoId = "";
        }

        public VideoIdFileName(string _fileName, string _videoId)
        {
            FileName = _fileName;
            VideoId = _videoId;
        }

        public VideoIdFileName(string _fileName)
        {
            FileName = _fileName;
        }
        public void AddVideoId(string _videoId)
        {
            VideoId = _videoId;
        }   

        public void AddFileName(string _filename)
        {
            FileName = _filename;
        }


    }
}
