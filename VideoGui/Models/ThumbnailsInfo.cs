using FirebirdSql.Data.FirebirdClient;

namespace VideoGui.Models
{
    public class ThumbnailsInfo
    {
        internal string title1;
        internal string title2;
        internal string sourceimage;
        internal string sourcedirectory;
        private FbDataReader reader;

        public ThumbnailsInfo(FbDataReader reader)
        {
            this.reader = reader;
        }

        public int TextOffset1 { get; internal set; }
        public int TextOffset2 { get; internal set; }
        public int TextSize1 { get; internal set; }
        public int TextSize2 { get; internal set; }
        public int Id { get; internal set; }

        internal bool IsChanged(ThumbnailCreator tncf)
        {
            return false;
        }
    }
}
