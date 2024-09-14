using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class CustomParams_RemoveTimeSpans
    {
        public int id { get; set; }
        public bool RemoveAll { get; set; }
        public CustomParams_RemoveTimeSpans(int _id, bool All = false)
        {
            id = _id;
            RemoveAll = All;
        }
    }

    public class CustomParams_EditTimeSpans
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Gap { get; set; }
        public int id { get; set; }
        public CustomParams_EditTimeSpans(int _id, TimeSpan _Start, TimeSpan _End, int _Gap)
        {
            id = _id;
            Start = _Start;
            End = _End;
            Gap = _Gap;
        }
    }


    public class CustomParams_Refresh
    {
    }
    public class CustomParams_AddTimeSpanEntries
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public int Gap { get; set; }
        public CustomParams_AddTimeSpanEntries(TimeSpan _Start, TimeSpan _End, int _Gap)
        {
            try
            {
                Start = _Start;
                End = _End;
                Gap = _Gap;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CustomParams_AddTimeSpanEntries {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }


    public class CustomParams_GetConnectionString()
    {
        public string ConnectionString = "";
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
