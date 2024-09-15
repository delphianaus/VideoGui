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
    public class ScheduleMapNames : INotifyPropertyChanged
    {
        private int _Id = -1;
        private string _Name = "";
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }
        public string Name { get => _Name; set { _Name = value; OnPropertyChanged(); } }
        public ScheduleMapNames(string _name, int id = -1)
        {
            try
            {
                Name = _name;
                Id = id;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleMapNames {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }

        public ScheduleMapNames(FbDataReader reader)
        {
            try
            {
                Id = reader["ID"].ToInt();
                Name = reader["NAME"].ToString();
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScheduleMapNames {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
