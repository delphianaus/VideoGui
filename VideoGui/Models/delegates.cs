using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace VideoGui.Models.delegates
{

    public enum ButtonReturnType { NotPresent, Disabled, Enabled };
    public enum FilterTypes { DestinationDirectory, DestinationFileName, SourceDirectory};
    public enum FilterClass { Current, Historic};
    public enum SortOrder { ASCENDING, DESCENDING };
    public enum AgeRestriction { AGE_RESTRICTION, NOT_AGE_RESTRICTED };
    public enum StatusTypes { HAS_SCHEDULE, DRAFT, UNLISTED, PRIVATE, PUBLIC };
    public enum TitleDesc { TITLE, DESCRIPTION };
    public enum ViewType { LESS_EQUAL, GREATER_EQUAL };
    public enum dataFormName { MasterTagSelect, TagSelecteditor };
    public enum dataUpdatType { Add, Remove, Edit, Change, Delete, Insert, Get };
    public enum ActionType { VideoSchedule, VideoUpload };
    //
    // Summary:
    //     Specifies the day of the week.
    public enum EventTypes {VideoUpload = 0, ShortsSchedule = 1, FullSchedule = 2,ScapeSchedule = 3, UploadTest = 4};
    public enum MadeForKids { MFK_SET_BY_YOU, MFK_SET_BY_YOUTUBE, NOT_MADE_FOR_KIDS, NO_SELECTION };
    public delegate void _StatsHandlerDateTimeSetter(string filename, DateTime start);
    public delegate DateTime _StatsHandlerDateTimeGetter(string filename);
    public delegate bool _StatsHandlerBool(int mode, string filename);
    public delegate void _AddConverterProgress(string Name, string Value);
    public delegate void _StatsHandlerExtra(string record, string filename);
    public delegate void _UpdateSpeed(string filename, float framess, float totalb, int framecalc, string Frames1080p);
    public delegate void _UpdateProgress(string filename, double Progress, TimeSpan Duration, TimeSpan Total);
    public delegate void _UpdateTime(int mode, string filename, TimeSpan eta);
    public delegate bool IsFileInUse(string filename, int threadid);
    public delegate void _StatsHandler(int mode, string filename);
    public delegate void CompairFinished();
    public delegate void ComplexFinished();

    public delegate string OnGetTagIds(int id);
    public delegate void GetNextNode();
    public delegate void ReportVideoScheduled(DateTime dateTime, string id, string title);
    public delegate void AudioJoinerOnClose();
    public delegate void AddressUpdate(string address);
    public delegate bool NodeUpdate(string Id, string Title, string Desc, string FileName, string status, Nullable<DateTime> dateTime);
    public delegate void AddressUpdateId(string address, string id);
    public delegate void AddRecordDelegate(bool IsElapsed,bool Is720P, bool IsShorts, int IsCreateShorts,bool IsTrimEncode, bool IsCutEncode, 
          bool IsDeleteMonitored, bool IsPersistantSource, bool IsAdobe, string textstart, string textduration, string sourcedirectory, 
          string destFilename, Nullable<DateTime> twitchschedule = null, string RTMP = "",bool IsTwichStream=false, bool IsMuxed=false,string MuxData="");
    public delegate void RemoveRecordDelegate(int ID,bool Bypass=false, bool KillRecord = false);
    public delegate void SetLists(int Id);
    public delegate void GetListDelegate(int ID);
    public delegate void ReOrderFiles(string txtsrcdir);
    public delegate void FileRenamerClear();
    public delegate void FileImporterClear();
    public delegate void ImportRecordAdd(string f1,string f2,TimeSpan t1);
    public delegate void SetFromTime(TimeSpan t);
    public delegate void SetToTime(TimeSpan t);
    public delegate void ClearTimes();
    public delegate bool CheckImports();
    public delegate void SetFilterAge(int a,int b);
    public delegate void OnKeyPressEvent(Key Key, string Data);
    public delegate (int,int) GetFilterAges();
    public delegate int GetTotalScheduled();
    public delegate IEnumerable<string> ListBoxConnect(string name);
    public delegate void OnPercentUpdate(long _percent);
    public delegate void OnFinish();
    public delegate bool OnFinishBool();
    public delegate void OnFinishId(int id);

    public delegate void OnStart(string SourceFileName);
    public delegate void OnAviDemuxStart(string SourceFileName);
    public delegate void OnAviDemuxEnd(string SourceFileName, string DestinationFile,int exitcode);
    public delegate void OnStop(string SourceFileName);
    public delegate void OnProgress(string Progress, string SourceFileName);
    public delegate void OnNotifyInsert(int count);
    public delegate void OnFinishByName(string name);
    public delegate int GetDataCount(string name);  
    public delegate string GetFilerString(FilterTypes Filters, FilterClass Active);
    public delegate void SetFilterString(string Filter, FilterTypes FilterType, FilterClass FilterClassIs);
    public delegate void OnFirebirdReader(FbDataReader reader);
    public delegate void databasehook<T>(object ThisForm, T tld);
    public delegate void dataupdate(dataUpdatType dt, int id, string description, object ThisForm);



}
