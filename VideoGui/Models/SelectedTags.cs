using FirebirdSql.Data.FirebirdClient;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VideoGui.Models
{
    public class SelectedTags : INotifyPropertyChanged
    {
        private int _Id, _SelectedTagId, _GroupTagId;

        private string _Description;
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public int SelectedTagId { get => _SelectedTagId; set { _SelectedTagId = value; OnPropertyChanged(); } }

        public int GroupTagId { get => _GroupTagId; set { _GroupTagId = value; OnPropertyChanged(); } }

        public string Description { get => _Description; set { _Description = value; OnPropertyChanged(); } }

        public SelectedTags(int __tagid, int __SelectedTagId, int __groupTagId, string __Description)
        {

            GroupTagId = __groupTagId;
            Id = __tagid;
            SelectedTagId = __SelectedTagId;
            Description = __Description;
        }


        public SelectedTags(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int id) ? id : -1;
                SelectedTagId = (reader["SELECTEDTAG"] is int STAG) ? STAG : -1;
                GroupTagId = (reader["GROUPTAGID"] is int GRP) ? GRP : -1;
                Description = (reader["TAG"] is string Desc) ? Desc : "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SelectedTags fbDatareader {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
