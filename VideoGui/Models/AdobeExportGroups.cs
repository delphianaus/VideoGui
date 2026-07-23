using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGui.Models
{
    public class AdobeExportGroups
    {
        public List<AdobeExport> exports = new List<AdobeExport>();

        public double StartingPoint = 0.0;
        public double Offset = 0.0;
        public string ExportName = "";

        public bool dummyrecord = false;
        public bool IsLastGroup = false;
        public AdobeExportGroups()
        {

        }

        public double StartPoint => StartingPoint + Offset;
        public double Duration => exports.FirstOrDefault()?.Duration ?? 0;
    }
}
