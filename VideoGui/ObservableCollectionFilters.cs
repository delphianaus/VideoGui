using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VideoGui.Models;
using VideoGui.Models.delegates;

namespace VideoGui
{
    public class ObservableCollectionFilters
    {
        public CollectionViewSource HistoricCollectionViewSource = new CollectionViewSource();
        public CollectionViewSource CurrentCollectionViewSource = new CollectionViewSource();
        public CollectionViewSource ImportCollectionViewSource = new CollectionViewSource();

        public bool ActiveCurrentCollection = false, ActiveHistoricCollection = false, ImportHistoricCollection = false;

        private GetListDelegate OnGetLists;

        private int HistoricMinAge = -1, HistoricMaxAge = -1;
        public string HistoricContainsSourceDirectory = "", HistoricContainsPath = "", HistoricContainsFileName = "";
        public string CurrentContainsSourceDirectory = "", CurrentContainsPath = "", CurrentContainsFileName = "";
        public TimeSpan FromTime = TimeSpan.Zero;
        public TimeSpan ToTime = TimeSpan.Zero;
        
        public void SetFromTimeSpan(TimeSpan time)
        {
            try
            {
                FromTime = time;
                ImportHistoricCollection = true;
                ImportCollectionViewSource.View.Refresh();
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private int FilterById = -1, FilterByTitleId = -1, FilterBySchedelingNameId = -1;
        public void SetToTimeSpan(TimeSpan time)
        {
            try
            {
                ToTime = time;
                ImportHistoricCollection = true;
                ImportCollectionViewSource.View.Refresh();
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void ClearImportTimes()
        {
            try
            {
                FromTime = TimeSpan.Zero;
                ToTime = TimeSpan.Zero;
                ImportHistoricCollection = true;
                ImportCollectionViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public (TimeSpan,TimeSpan) GetTimeSpans()
        {
            try
            {
                return (FromTime,ToTime);
            }
            catch(Exception ex) 
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return (TimeSpan.Zero,TimeSpan.Zero);
            }
        }
        public string GetFilterString(FilterTypes Filter, FilterClass Active)
        {
            try
            {
                return (Active == FilterClass.Current) ? ((Filter == FilterTypes.DestinationFileName) ? CurrentContainsFileName :
                    (Filter == FilterTypes.SourceDirectory) ? CurrentContainsSourceDirectory :
                    (Filter == FilterTypes.DestinationDirectory) ? CurrentContainsPath : "") :
                    ((Filter == FilterTypes.DestinationFileName) ? HistoricContainsFileName :
                    (Filter == FilterTypes.SourceDirectory) ? HistoricContainsSourceDirectory :
                    (Filter == FilterTypes.DestinationDirectory) ? HistoricContainsPath : "");
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return "";
            }
        }
        
        public (int,int) GetFilterAges()
        {
            try
            {
                return (HistoricMinAge, HistoricMaxAge);
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
                return (-1, -1);
            }
        }
        public void SetHistoricContainsPath(string path)
        {
            try
            {
                HistoricContainsPath = path;
                ActiveHistoricCollection = true;
                HistoricCollectionViewSource.View.Refresh();
            }
            catch(Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SetCurrentContainsPath(string path)
        {
            try
            {
                CurrentContainsPath = path;
                ActiveCurrentCollection = true;
                CurrentCollectionViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SetHistoricContainsFileName(string value)
        {
            try
            {
                HistoricContainsFileName = value;
                ActiveHistoricCollection = true;
                HistoricCollectionViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SetCurrentContainsFileName(string value)
        {
            try
            {
                CurrentContainsFileName = value;
                ActiveCurrentCollection = true;
                if (CurrentCollectionViewSource.Source != null)
                {
                    CurrentCollectionViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SetHistoricContainsSourceDirectory(string value)
        {
            try
            {
                HistoricContainsSourceDirectory = value;
                ActiveHistoricCollection = true;
                HistoricCollectionViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        public void SetCurrentContainsSourceDirectory(string value)
        {
            try
            {
                CurrentContainsSourceDirectory = value;
                ActiveCurrentCollection = true;
                CurrentCollectionViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public void SetHistoricAges(int min, int max)
        {
            try
            {
                HistoricMinAge = min;
                HistoricMaxAge = max;
                ActiveHistoricCollection = true;
                HistoricCollectionViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        public ObservableCollectionFilters()
        {
            try
            {
                CurrentCollectionViewSource.Filter += new FilterEventHandler(OnCurrentCollectionFilter);
                HistoricCollectionViewSource.Filter += new FilterEventHandler(OnHistoricCollectionFilter);
                ImportCollectionViewSource.Filter += new FilterEventHandler(OnImportCollectionFilter);

                HistoricCollectionViewSource.IsLiveFilteringRequested = true;
                CurrentCollectionViewSource.IsLiveFilteringRequested = true;

            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }

        private void OnTitleTagViewFilter(object sender, FilterEventArgs e)
        {
            try
            {
                if (FilterByTitleId == -1)
                {
                    e.Accepted = true;
                }
                else
                {
                    if (e.Item is TitleTags seltag)
                    {
                        e.Accepted = (seltag.GroupId == FilterByTitleId);
                        if (e.Accepted)
                        {
                            if (true)
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"OnTitleTagViewFilter {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
            }
        }


        private void OnImportCollectionFilter(object sender, FilterEventArgs e)
        {
            try
            {
                if (ImportHistoricCollection)
                {
                    if (e.Item is FileInfoGoPro fgp)
                    {
                        e.Accepted = fgp.TimeData.IfBetweenTimeSpans(FromTime, ToTime);
                    }
                    else e.Accepted = true;
                }
                else e.Accepted = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnHistoricCollectionFilter(object sender, FilterEventArgs e)
        {
            try
            {
                if (ActiveHistoricCollection)
                {
                    //filter code here
                    if (e.Item is ComplexJobHistory cjh)
                    {
                        e.Accepted = cjh.RecordAge.ToInt().IfBetweenInts(HistoricMinAge, HistoricMaxAge) &&
                             cjh.SourceDirectory.IfContains(HistoricContainsSourceDirectory) &&
                             cjh.DestinationDirectory.IfContains(HistoricContainsPath) &&
                             cjh.DestinationFile.IfContains(HistoricContainsFileName);
                    }

                    else e.Accepted = true;
                }
                else e.Accepted = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
        private void OnCurrentCollectionFilter(object sender, FilterEventArgs e)
        {
            try
            {
                if (ActiveCurrentCollection)
                {
                    if (e.Item is ComplexJobList cjh)
                    {
                        e.Accepted = cjh.SourceDirectory.IfContains(CurrentContainsSourceDirectory) &&
                             cjh.DestinationDirectory.IfContains(CurrentContainsPath) &&
                             cjh.DestinationFile.IfContains(CurrentContainsFileName);
                    }
                    else e.Accepted = true;
                }
                else e.Accepted = true;
            }
            catch (Exception ex)
            {
                ex.LogWrite(MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
