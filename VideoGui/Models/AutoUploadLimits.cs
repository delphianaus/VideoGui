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
    public class AutoUploadLimits : INotifyPropertyChanged
    {
        private int _Id = -1, _Limit = 0;
        private DateOnly _LimitDate;
        private bool _LimitActive = false;

        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        public int Limit { get => _Limit; set { _Limit = value; OnPropertyChanged(); } }
        public DateOnly LimitDate { get => _LimitDate; set { _LimitDate = value; OnPropertyChanged(); } }
        public bool LimitActive { get => _LimitActive; set { _LimitActive = value; OnPropertyChanged(); } }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public AutoUploadLimits(FbDataReader reader)
        {
            try
            {
                Id = (reader["Id"] is int idx) ? idx : -1;
                LimitDate = (reader["LIMITDATE"] is DateTime drr) ? DateOnly.FromDateTime(drr) : new DateOnly();
                LimitActive = (reader["LIMITACTIVE"] is Int16 ir) ? (ir == 1) : false;
                Limit = (reader["LIMIT"] is int ird) ? ird : 0;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AutoUploadLimits CTOR {MethodBase.GetCurrentMethod().Name} {ex.Message} {this}");
            }
        }

        public AutoUploadLimits(int _Id, int _Limit, DateOnly _LimitDate, bool _IsActive)
        {
            Id = _Id;
            Limit = _Limit;
            LimitDate = _LimitDate;
            LimitActive = _IsActive;
        }


    }
}
