using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VideoGui.Models.delegates;

namespace VideoGui.Models
{
    
    public class CustomParams_InsertTags
    {
        public List<int> TagIds { get; set; } = new List<int>();
        public int GroupId { get; set; } = -1;
        public CustomParams_InsertTags(List<int> _TagIds, int _groupid)
        {
            TagIds.AddRange(_TagIds);
            GroupId = _groupid;
        }
        public CustomParams_InsertTags()
        {

        }
    }
    public class CustomParams_Select
    {
        public int Id { get; set; } = -1;
        public CustomParams_Select(int _id)
        {
            Id = _id;
        }
    }
    public class CustomParams_Save
    {
        public int id { get; set; } = -1;
        public string Name { get; set; } = "";
        public CustomParams_Save(string name, int _id = -1)
        {
            id = _id;
            Name = name;
        }
    }
    public class CustomParams_DescSelect
    {
        public ShortsDirectory UploadsReleaseInfo;
        public CustomParams_DescSelect(ShortsDirectory _UploadsReleaseInfo)
        {
            UploadsReleaseInfo = _UploadsReleaseInfo;
        }
    }
    public class CustomParams_Delete
    {
        public string Name { get; set; } = "";
        public dataUpdatType dataUpdatType { get; set; } = dataUpdatType.Delete;
        public int Id { get; set; } = -1;
        public CustomParams_Delete(int _id, string _Name)
        {
            Name = _Name;
            Id = _id;
        }
    }


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

    public class CustomParams_SetTimeSpans
    {
        public Nullable<DateTime> Start { get; set; }
        public Nullable<DateTime> End { get; set; }
        public CustomParams_SetTimeSpans(Nullable<DateTime> _Start, Nullable<DateTime> _End)
        {
            Start = _Start;
            End = _End;
        }
    }

    public class CustomParams_Get
    {
        public dataUpdatType dataUpdatType { get; set; } = dataUpdatType.Get;
        public string data_string { get; set; } = "";
        public int Id { get; set; } = -1;

        public int Idx { get; set; } = -1;
        public CustomParams_Get(int _id, string datastring, int _idx = -1)
        {
            data_string = datastring;
            Id = _id;
            Idx = _idx;
        }

        
        public CustomParams_Get()
        {

        }
    }

    public class CustomParams_EditName
    {
        public string name { get; set; } = "";
        public int id { get; set; } = -1;

        public CustomParams_EditName(int _id, string _name)
        {
            id = _id;
            name = _name;
        }
    }

    public class CustomParams_Initialize
    {
        bool IsUploads { get; set; }

        public CustomParams_Initialize(bool _IsUploads = false)
        {
            IsUploads = _IsUploads;
        }
    }

    public class CustomParams_TitleSelect
    {
        public ShortsDirectory UploadsReleaseInfo;
        public CustomParams_TitleSelect(ShortsDirectory _UploadsReleaseInfo)
        {
            UploadsReleaseInfo = _UploadsReleaseInfo;
        }
    }
    public class CustomParams_Finish
    {
        public string name { get; set; } = "";
        public CustomParams_Finish(string name)
        {
            this.name = name;
        }
    }

    public class CustomParams_GetTitle
    {
        public string name { get; set; } = "";
        public int title { get; set; } = -1;
        public int id { get; set; } = -1;

        public CustomParams_GetTitle(int _id, int _title)
        {
            id = _id;
            title = _title;
        }
    }
    public class CustomParams_GetDesc
    {
        public string name { get; set; } = "";
        public int desc { get; set; } = -1;
        public int id { get; set; } = -1;

        public CustomParams_GetDesc(int _id, int _desc)
        {
            id = _id;
            desc = _desc;
        }
    }
    public class CustomParams_Remove
    {
        public string Name { get; set; } = "";
        public dataUpdatType dataUpdatType { get; set; } = dataUpdatType.Remove;
        public int id { get; set; } = -1;
        string data_string { get; set; } = "";
        public int TitleLength { get; set; } = -1;
        public CustomParams_Remove(int _id, string datastring = "")
        {
            id = _id;
            data_string = datastring;
        }
    }

    public class CustomParams_AddVideoInfo
    {

        public int id = -1, groupid = -1;
        public bool isfixed = false;
        public StatusTypes Status { get; set; } = StatusTypes.PRIVATE;
        public string videoid = "", filename = "", title = "", TableName = "";
        public CustomParams_AddVideoInfo(Nullable<int> _id, StatusTypes _status, string _videoid, string _title, 
            string _filename, int _groupid, bool _isfixed, string _TablwName= "")
        {
            id = (_id.HasValue) ? _id.Value : id;
            groupid = _groupid;
            Status = _status;
            title = _title;
            TableName = _TablwName;
            filename = _filename;
            videoid = _videoid;
            isfixed = _isfixed;
        }

    }
    public class CustomParams_AddDescription
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public CustomParams_AddDescription(int _id, string _Name, string _Description)
        {
            Id = _id;
            Name = _Name;
            Description = _Description;
        }
    }
    public enum UpdateType { Title, Description, Tag };
    public class CustomParams_Update
    {
        public int id { get; set; }

        public string DirectoryName { get; set; }

        public UpdateType updatetype { get; set; } = UpdateType.Title;
        public CustomParams_Update(int _id, UpdateType _updatetype = UpdateType.Title, string _DirectoryName = "")
        {
            id = _id;
            updatetype = _updatetype;
            DirectoryName = _DirectoryName;
        }
    }
    public class CustomParams_Add
    {
        public string Name { get; set; } = "";
        public dataUpdatType dataUpdatType { get; set; } = dataUpdatType.Add;
        public string data_string { get; set; } = "";
        public CustomParams_Add(string _Name, string datastring)
        {
            Name = _Name;
            data_string = datastring;
        }
    }
    public class CustomParams_Refresh
    {
    }

    public class CustomParams_Wait
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

    public class CustomParams_InsertIntoTable
    {
        public string id = "";
        public string filename = "";
        public CustomParams_InsertIntoTable(string _id, string _filename)
        {
            id = _id;
            filename = _filename;
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
