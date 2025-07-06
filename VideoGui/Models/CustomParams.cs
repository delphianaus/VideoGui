using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using VideoGui.Models.delegates;
using Windows.ApplicationModel.Background;

namespace VideoGui.Models
{
    public class CustomParams_UpdateMultiShortsInfo
    {
        public int linkedId = 0;
        public int numberofShorts = 0;
        public DateTime lastTimeUploaded = DateTime.Now.Date.AddYears(-100);
        public string uploaddir = "";

        public CustomParams_UpdateMultiShortsInfo(int linkedId, int numberofShorts, string uploaddir)
        {
            this.linkedId = linkedId;
            this.numberofShorts = numberofShorts;
            this.uploaddir = uploaddir;
        }

        public CustomParams_UpdateMultiShortsInfo(int linkedId, int numberofShorts, DateTime lastTimeUploaded, string uploaddir)
        {
            this.linkedId = linkedId;
            this.numberofShorts = numberofShorts;
            this.lastTimeUploaded = lastTimeUploaded;
            this.uploaddir = uploaddir;
        }
    }
    public class CustomParams_SetIndex
    {
        public int index = 0;
        public CustomParams_SetIndex(int index)
        {
            this.index = index;
        }
    }
    public class CustomParams_LookUpId
    {
        public string DirectoryName = "";
        public CustomParams_LookUpId(string DirectoryName)
        {
            this.DirectoryName = DirectoryName;
        }
    }
    public class CustomParams_LookUpTitleId
    {
        public string DirectoryName = "";
        public CustomParams_LookUpTitleId(string DirectoryName)
        {
            this.DirectoryName = DirectoryName;
        }
    }
    public class CustomParams_InsertIntoShortsDirectory
    {
        public string DirectoryName = "";
        public CustomParams_InsertIntoShortsDirectory(string DirectoryName)
        {
            this.DirectoryName = DirectoryName;
        }
    }
    public class CustomParams_UpdateMultishortsByDir
    {
        public string DirectoryName = "";
        public string ParentDirectory = "";
        public CustomParams_UpdateMultishortsByDir(string DirectoryName, string parentDirectory)
        {
            this.DirectoryName = DirectoryName;
            ParentDirectory = parentDirectory;
        }
    }

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

    public class CustomParams_SelectById
    {
        public int Id { get; set; } = -1;
        public string VideoId { get; set; } = "";
        public CustomParams_SelectById(string _videoId)
        {
            VideoId = _videoId;
        }
    }
    public class CustomParams_RematchedUpdate
    {
        public int newid { get; set; } = -1;
        public string directory { get; set; } = "";
        public CustomParams_RematchedUpdate(int _newid, string _directory)
        {
            newid = _newid;
            directory = _directory;
        }
    }
    public class CustomParams_RematchedLookup
    {
        public int oldId { get; set; } = -1;
        public CustomParams_RematchedLookup(int _Oldid)
        {
            oldId = _Oldid;
        }
    }
    public class CustomParams_UpdateStats
    {
        public string DirectoryName = "";
        public CustomParams_UpdateStats(string DirectoryName)
        {
            this.DirectoryName = DirectoryName;
        }
    }

    public class CustomParams_UpdateUploadsRecords
    {
        public string ParentDirectory { get; set; } = "";
        public List<string> DirectoryName = new();
        public CustomParams_UpdateUploadsRecords(List<string> DirectoryName, string ParentDirectory)
        {
            this.DirectoryName = DirectoryName;
            this.ParentDirectory = ParentDirectory;

        }
    }

    public class CustomParams_GetUploadsRecCnt
    {
        public bool IsLast24Hours { get; set; } = false;    
        public CustomParams_GetUploadsRecCnt(bool IsLast24Hours)
        {
            this.IsLast24Hours = IsLast24Hours;
        }
    }
    public class CustomParams_UpdateTitleById
    {
        public int Id { get; set; } = -1;
        public int Title { get; set; } = -1;
        public int LinkedId { get; set; } = -1;
        public CustomParams_UpdateTitleById(int _id, int _title, int _LinkedId = -1)
        {
            Id = _id;
            Title = _title;
            LinkedId = _LinkedId;
        }
    }

    public class CustomParams_UpdateDescById
    {
        public int Id { get; set; } = -1;
        public int Desc { get; set; } = -1;
        public int LinkedId { get; set; } = -1;
        public CustomParams_UpdateDescById(int _id, int _desc, int _LinkedId = -1)
        {
            Id = _id;
            Desc = _desc;
            LinkedId = _LinkedId;
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

    public class CustomParams_DataSelect
    {
        public int Id { get; set; } = -1;
        public CustomParams_DataSelect(int _id)
        {
            Id = _id;
        }
    }
    public class CustomParams_SaveSchedule
    {
        public DateTime ScheduleDate { get; set; }
        public TimeSpan ScheduleTimeStart { get; set; }
        public TimeSpan ScheduleTimeEnd { get; set; }
        public bool TestMode { get; set; } = false;
        public int max { get; set; } = 0;
        public CustomParams_SaveSchedule(DateTime _ScheduleDate, TimeSpan _ScheduleTimeStart,
            TimeSpan _ScheduleTimeEnd, int _max, bool _TestMode)
        {
            ScheduleDate = _ScheduleDate;
            ScheduleTimeStart = _ScheduleTimeStart;
            ScheduleTimeEnd = _ScheduleTimeEnd;
            max = _max;
            TestMode = TestMode;
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

    public class CustomParams_InsertMultiShortsInfo
    {
        public int numberofShorts;
        public int linkedId;
        public DateTime lastTimeUploaded;
        public bool IsActive;

        public CustomParams_InsertMultiShortsInfo(int numberofShorts, int linkedId, DateTime lastTimeUploaded, bool v)
        {
            this.numberofShorts = numberofShorts;
            this.linkedId = linkedId;
            this.lastTimeUploaded = lastTimeUploaded;
            this.IsActive = v;
        }
    }
    public class CustomParams_GetBaseDirectory
    {
        public string User { get; set; } = "";
        public bool found { get; set; } = false;
        public string basedir { get; set; } = "";
        public CustomParams_GetBaseDirectory(string _user)
        {
            User = _user;
        }
    }
    public class CustomParams_Authorize
    {
        public bool IsUser { get; set; } = true;
        public string data { get; set; } = "";
        public bool Authorized { get; set; } = false;
        public CustomParams_Authorize(string data, bool IsUser = false)
        {
            this.data = data;
            this.IsUser = IsUser;
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


    public class CustomParams_UpdateAction
    {
        public int id { get; set; }
        public Nullable<DateTime> ActionDate { get; set; }
        public Nullable<DateOnly> ScheduleDate { get; set; }
        public Nullable<TimeSpan> ScheduleTimeStart { get; set; }
        public Nullable<TimeSpan> ScheduleTimeEnd { get; set; }
        public Nullable<DateTime> CompletedDate { get; set; }
        public string ScheduleName { get; set; } = "";
        public string ActionName { get; set; } = "";

        public int Max { get; set; } = 0;
        public CustomParams_UpdateAction(int _id, Nullable<DateTime> _ActionDate,
            Nullable<DateOnly> _ScheduleDate, Nullable<TimeSpan> _ScheduleTimeStart,
            Nullable<TimeSpan> _ScheduleTimeEnd, Nullable<DateTime> _CompletedDate,
            string _ScheduleName, string _ActionName, int _max)
        {
            id = _id;
            ActionDate = _ActionDate;
            ScheduleDate = _ScheduleDate;
            ScheduleTimeStart = _ScheduleTimeStart;
            ScheduleTimeEnd = _ScheduleTimeEnd;
            CompletedDate = _CompletedDate;
            ScheduleName = _ScheduleName;
            ActionName = _ActionName;
            Max = _max;
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

    public class CustomParams_SetFilterId
    {
        public int FilterId { get; set; } = 0;
        public CustomParams_SetFilterId(int _FilterId)
        {
            FilterId = _FilterId;
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
        bool IsUploads { get; set; } = false;
        public int Id { get; set; } = -1;

        public CustomParams_Initialize(int _id)
        {
            Id = _id;
        }

        public CustomParams_Initialize(bool _IsUploads = false, int _Id = -1)
        {
            IsUploads = _IsUploads;
            Id = _Id;
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
            string _filename, int _groupid, bool _isfixed, string _TablwName = "")
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

    public class CustomParams_GetDirectory
    {
        public string DirectoryName { get; set; } = "";
        public CustomParams_GetDirectory(string _DirectoryName)
        {
            DirectoryName = _DirectoryName;
        }
    }

    public class CustomParams_GetDescIdByDirectory
    {
        public string DirectoryName { get; set; } = "";
        public CustomParams_GetDescIdByDirectory(string _DirectoryName)
        {
            DirectoryName = _DirectoryName;
        }
    }
    public class CustomParams_DescUpdate
    {
        public string DirectoryName { get; set; } = "";
        public string Description { get; set; } = "";
        public CustomParams_DescUpdate(string _DirectoryName, string _Description)
        {
            DirectoryName = _DirectoryName;
            Description = _Description;
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

    public class CustomParams_GetCurrentDescId
    {
        public int Id { get; set; } = -1;
        public CustomParams_GetCurrentDescId(int _id)
        {
            Id = _id;
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

    public class CustomParams_TitleDesc
    {
        string id = "";
    }
    public class CustomParams_AddDirectory
    {
        public string DirectoryName = "";
        public CustomParams_AddDirectory(string _DirectoryName)
        {
            DirectoryName = _DirectoryName;
        }
    }

    public class CustomParams_GetShortsDirectoryById
    {
        public int Id = -1;
        public CustomParams_GetShortsDirectoryById(int _id)
        {
            Id = _id;
        }
    }

    public class CustomParams_RemoveSelectedDirectory
    {
        public string DirectoryName = "";
        public int Id = -1;
        public CustomParams_RemoveSelectedDirectory(int _id, string _DirectoryName)
        {
            Id = _id;
            DirectoryName = _DirectoryName;
        }
    }
    public class CustomParams_MoveDirectory
    {
        public string DirectoryName = "";
        public CustomParams_MoveDirectory(string _DirectoryName)
        {
            DirectoryName = _DirectoryName;
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
