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
        private int _Id = -1, _LinkedShortsDirectoryId = -1, _NumberOfShorts = 0;
        private string _ShortsDir = "";
        private DateTime LastUploadedDate = DateTime.Now.Date.AddYears(-100);
        private bool _IsActive = false;
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public string ShortsDir { get => _ShortsDir; set { _ShortsDir = value; OnPropertyChanged(); } }
        public int NumberOfShorts { get => _NumberOfShorts; set { _NumberOfShorts = value; OnPropertyChanged(); } }
        public int LinkedShortsDirectoryId { get => _LinkedShortsDirectoryId; set { _LinkedShortsDirectoryId = value; OnPropertyChanged(); } }
        public string LastUploadedFile { get => GetUploadedDateString(); set { OnPropertyChanged(); } }
        public DateTime LastUploadedDateFile { get => LastUploadedDate; set { LastUploadedDate = value; OnPropertyChanged(); } }
        public bool IsShortActive { get => _IsActive; set { _IsActive = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public string GetUploadedDateString()
        {
            return (LastUploadedDate.Year < 2000) ?  "" : LastUploadedDate.ToString(@"dd.MM.yyyy");
        }   
        public SelectedShortsDirectories(int _Id, string _ShortsDir,
            int _LinkedShortsDirectoryId,
            int _NumberOfShorts, bool _IsActive, DateTime _LastUploadedDate)
        {
            Id = _Id;
            ShortsDir = _ShortsDir;
            IsShortActive = _IsActive;
            LinkedShortsDirectoryId = _LinkedShortsDirectoryId;
            NumberOfShorts = _NumberOfShorts;
            LastUploadedDateFile = _LastUploadedDate;
        }
        public SelectedShortsDirectories(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int id) ? id : -1;
                LinkedShortsDirectoryId = (reader["LINKEDSHORTSDIRECTORYID"] is int grp) ? grp : -1;
                NumberOfShorts = (reader["NUMBEROFSHORTS"] is int GRP) ? GRP : -1;
                ShortsDir = (reader["SHORTSDIR"] is string Desc) ? Desc : "";
                IsShortActive = (reader["ISACTIVE"] is short IsActive) ? IsActive == 1 : false;
                LastUploadedDate = (reader["LASTUPLOADEDDATE"] is DateTime date) ? date : DateTime.Now.Date.AddYears(-100);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SelectedTags fbDatareader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
    }
}


