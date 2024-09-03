using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class CustomParams_GetVideoFileName
    {
        public string id = "";
        public string filename = "";
        public bool Unlisted = false;
        public bool found => filename != "";
        public CustomParams_GetVideoFileName(string _id, bool _Unlisted)
        {
            id = _id;
            Unlisted = _Unlisted;
        }

    }
    public class CustomParmam_NewVideoInfo
    {
        public string id = "";
        public string title = "";
        public bool unlisted = false;
        public CustomParmam_NewVideoInfo(string _title, string _id, bool _unlisted = false)
        {
            id = _id;
            title = _title;
            unlisted = _unlisted;
        }

    }

    public class CustomParams_InsertWithId
    {
        public int id { get; set; }
        public int Groupid { get; set; }

        public bool Unlisted = false;
        public string FileName { get; set; }
        public string VideoId { get; set; }
        public int TitleLength { get; set; } = 0;
        public CustomParams_InsertWithId(string _Id, string _filename, bool unlisted = false)
        {
            VideoId = _Id;
            FileName = _filename;
            Unlisted = unlisted;
        }

        public CustomParams_InsertWithId(int _Id, int _groupid)
        {
            id = _Id;
            Groupid = _groupid;
        }
    }
}
