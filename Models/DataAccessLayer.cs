using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml.Schema;
using Nancy.Routing.Constraints;
using System.Windows.Forms;
using System.Security.AccessControl;

namespace VideoGui
{

    public class Schedule
    {
        public Schedule()
        {

        }
        public string time { get; set; }
        public List<string> days { get; set; }
    }

    public class Rating
    {
        public Rating()
        {

        }
        public double? average { get; set; }
    }

    public class Country
    {
        public Country()
        {

        }
        public string name { get; set; }
        public string code { get; set; }
        public string timezone { get; set; }
    }

    public class Network
    {
        public Network()
        {

        }
        public int id { get; set; }
        public string name { get; set; }
        public Country country { get; set; }
    }

    public class WebChannel
    {
        public WebChannel()
        {

        }
        public int id { get; set; }
        public string name { get; set; }
        public Country country { get; set; }
    }

    public class Externals
    {
        public Externals()
        {
            imdb = "";
        }
        public int? tvrage { get; set; }
        public int? thetvdb { get; set; }
        public string imdb { get; set; }
    }

    public class Image
    {
        public Image()
        {

        }
        public string medium { get; set; }
        public string original { get; set; }
    }

    public class Self
    {
        public Self()
        {

        }
        public string href { get; set; }
    }

    public class Previousepisode
    {
        public Previousepisode()
        {

        }
        public string href { get; set; }
    }

    public class Nextepisode
    {
        public Nextepisode()
        {

        }
        public string href { get; set; }
    }

    public class Links
    {
        public Links()
        {

        }
        public Self self { get; set; }
        public Previousepisode previousepisode { get; set; }
        public Nextepisode nextepisode { get; set; }
    }

    public class TVShows
    {
        public TVShows()
        {

        }
        public int id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string language { get; set; }
        public List<string> genres { get; set; }
        public string status { get; set; }
        public int? runtime { get; set; }
        public string premiered { get; set; }
        public string officialSite { get; set; }
        public Schedule schedule { get; set; }
        public Rating rating { get; set; }
        public int weight { get; set; }
        public Network network { get; set; }
        public WebChannel webChannel { get; set; }
        
        public Externals externals { get; set; }
        public Image image { get; set; }
        public string summary { get; set; }
        public int updated { get; set; }
        public Links _links { get; set; }
        public object dvdCountry { get; set; }
    }

    public class CutListToFrom
    {
        public System.TimeSpan From { get; set; }
        public System.TimeSpan To { get; set; }
        public CutListToFrom(System.TimeSpan _From, System.TimeSpan _To)
        {
            To = _To;
            From = _From;
        }
    }

    public class FileSearcher
    {
        public readonly string TVShowName;
        public readonly List<string> TVSearchNames;
        public FileSearcher(string _TVShowName)
        {
            TVShowName = _TVShowName;
            TVSearchNames = new();
        }

        public void AddSeach(string newsearch)
        {
            if (TVSearchNames.IndexOf(newsearch) == -1)
            {
                TVSearchNames.Add(newsearch);
            }

        }
    }
}
