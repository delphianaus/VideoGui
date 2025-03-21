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
    public class Rematched : INotifyPropertyChanged
    {
        private int _id, _oldId, _newId;
        private string _title;

        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public int OldId { get => _oldId; set { _oldId = value; OnPropertyChanged(); } }
        public int NewId { get => _newId; set { _newId = value; OnPropertyChanged(); } }
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }

        public Rematched(int _id, int _oldId, int _newId, string _title)
        {
            Id = _id;
            OldId = _oldId;
            NewId = _newId;
            Title = _title;
        }

        public Rematched() { }

        public Rematched(FbDataReader r)
        {
            try
            {
                Id = (r["ID"] is int id) ? id : -1;
                OldId = (r["OLDID"] is int oldId) ? oldId : -1;
                NewId = (r["NEWID"] is int newId) ? newId : -1;
                Title = (r["DIRECTORY"] is string title) ? title : "";
            }
            catch(Exception ex) 
            {
                ex.LogWrite($"Rematched {MethodBase.GetCurrentMethod()?.Name} {ex.Message} {ex.StackTrace}");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
