using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class SelectedShortsDirectories : INotifyPropertyChanged
    {
        private int _Id = -1, _LinkedShortsDirectoryId = -1, _NumberOfShorts = 0, _TitleId = -1, _DescId = -1;
        private string _DirectoryName = "", _LinkedDescId = "" , _LinkedTitleId = "";      
        private DateTime _LastUploadedDate = DateTime.Now.Date.AddYears(-100);
        private bool _IsActive = false;

        public string LinkedDescId { get => _LinkedDescId; set { _LinkedDescId = value;GetLinkedDescId(); OnPropertyChanged(); } }
        public string LinkedTitleId { get => _LinkedTitleId; set { _LinkedTitleId = value; GetLinkedTitle(); OnPropertyChanged(); } }
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public int TitleId { get => _TitleId; set { _TitleId = value; OnPropertyChanged(); } }
        public int DescId { get => _DescId; set { _DescId = value; OnPropertyChanged(); } }
        public string DirectoryName { get => _DirectoryName; set { _DirectoryName = value; OnPropertyChanged(); } }
        public int NumberOfShorts { get => _NumberOfShorts; set { _NumberOfShorts = value; OnPropertyChanged(); } }
        public int LinkedShortsDirectoryId { get => _LinkedShortsDirectoryId; set { _LinkedShortsDirectoryId = value; OnPropertyChanged(); } }
        public string LastUploadedFile { get => GetUploadedDateString(); set { OnPropertyChanged(); } }
        public DateTime LastUploadedDateFile { get => _LastUploadedDate; set { _LastUploadedDate = value; OnPropertyChanged(); } }
        
        public bool IsTitleAvailable { get => LinkedTitleId.Length > 0; set {; OnPropertyChanged(); } }
        public bool IsDescAvailable { get => LinkedDescId.Length > 0; set {; OnPropertyChanged(); } }
        public bool IsShortActive { get => _IsActive; set { _IsActive = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
           
        }
        public string GetUploadedDateString()
        {
            return (_LastUploadedDate.Year < 2000) ?  "" : _LastUploadedDate.ToString(@"dd.MM.yyyy");
        }   
        public SelectedShortsDirectories(int _Id, string _DirectoryName, bool _IsActive,
            int _LinkedShortsDirectoryId, int _NumberOfShorts, DateTime _LastUploadedDate)
        {
            Id = _Id;
            DirectoryName = _DirectoryName;
            IsShortActive = _IsActive;
            LinkedShortsDirectoryId = _LinkedShortsDirectoryId;
            NumberOfShorts = _NumberOfShorts;
            LastUploadedDateFile = _LastUploadedDate;
        }

        public void GetLinkedTitle()
        {
            if (LinkedTitleId is not null)
            {
                IsTitleAvailable = LinkedTitleId.Length > 0;
            }
        }

        public void GetLinkedDescId()
        {
            if (LinkedDescId is not null)
            {
                IsDescAvailable = LinkedDescId.Length > 0;
            }
        }
        public SelectedShortsDirectories(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int id) ? id : -1;
                LinkedShortsDirectoryId = (reader["LINKEDSHORTSDIRECTORYID"] is int grp) ? grp : -1;
                NumberOfShorts = (reader["NUMBEROFSHORTS"] is int GRP) ? GRP : -1;
                DirectoryName = (reader["DIRECTORYNAME"] is string Desc) ? Desc : "";
                IsShortActive = (reader["ISSHORTSACTIVE"] is short IsActive) ? IsActive == 1 : false;
                LastUploadedDateFile = (reader["LASTUPLOADEDDATE"] is DateTime date) ? date : DateTime.Now.Date.AddYears(-100);
                DescId = (reader["DESCID"] is int descid) ? descid : -1;
                TitleId = (reader["TITLEID"] is int titleid) ? titleid : -1;
                LinkedDescId = (reader["LINKEDDESCIDS"] is string linkedDescId) ? linkedDescId : "";
                LinkedTitleId = (reader["LINKEDTITLEIDS"] is string linkedTitleId) ? linkedTitleId : "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SelectedTags fbDatareader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}


