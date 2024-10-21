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
    public class AppliedSchedules : INotifyPropertyChanged
    {

        private int _id = -1, _Days = 0;
        private string _Name = "";
        public int Id { get => _id; set { _id = value; OnPropertyChanged(); } }
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }

        public AppliedSchedules(int ID, string Name)
        {
            try
            {
                this._id = ID;
                this._Name = Name;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AppliedSchedules {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }

        public AppliedSchedules(FbDataReader reader)
        {
            try
            {
                Id = (reader["ID"] is int ID) ? ID : -1;
                Name = (reader["NAME"] is string NAME) ? NAME : "";
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AppliedSchedules reader {this} {MethodBase.GetCurrentMethod().Name} {ex.Message}");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
