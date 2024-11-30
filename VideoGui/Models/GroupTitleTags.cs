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
    class GroupTitleTags : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private string _tags = string.Empty;
        public string _ids = string.Empty;
        private int _GroupId = -1;


        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        public string Tags
        {
            get { return _tags; }
            set { _tags = value; OnPropertyChanged(); }
        }
        public int GroupId
        {
            get { return _GroupId; }
            set { _GroupId = value; OnPropertyChanged(); }
        }

        public string Ids
        {
            get { return _ids; }
            set { _ids = value; OnPropertyChanged(); }
        }


        public GroupTitleTags()
        {

        }
        public GroupTitleTags(FbDataReader reader)
        {
            try
            {
                Name = (reader["NAME"] is string name) ? name : string.Empty;
                Tags = (reader["TAGS"] is string desc) ? desc : string.Empty;
                GroupId = (reader["GROUPID"] is int id) ? id : -1;
                Ids = (reader["IDS"] is string ids) ? ids : string.Empty;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GroupTitleTags {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
