using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class IdhasUpdated
    {
        public int id { get; set; } =-1;
        public bool hasUpdated { get; set; } = false;
        public IdhasUpdated(int _id, bool _hasUpdated = false)
        {
            id = _id;
            hasUpdated = _hasUpdated;
        }
    }
}
