using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VideoGui.Models.delegates;

namespace VideoGui.Models
{
    public class ConverterProgress
    {
        public List<ConverterList> converterLists = new List<ConverterList>();
        public void RemoveName(string _Name)
        {
            try
            {
                for (int i = 0; i < converterLists.Count; i++)
                {
                    if (converterLists[i].DestName == _Name)
                    {
                        converterLists.RemoveAt(i);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                ex.LogWrite($"ConverterProgress.RemoveName {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public int GetCount(string _Name)
        {
            try
            {
                for (int i = 0; i < converterLists.Count; i++)
                {
                    if (converterLists[i].DestName == _Name)
                    {
                        return converterLists[i].ProgressList.Count;
                        break;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterProgress.GetCount {MethodBase.GetCurrentMethod().Name}");
                return -1;
            }
        }
       
        
        public _AddConverterProgress AddNewProgressEventHandler(string _Name)
        {
            try
            {
                bool found = false;
                foreach (var _ in converterLists.Where(converter => converter.DestName == _Name).Select(converter => new { }))
                {
                    found = true;
                    break;
                }

                if (!found)
                {
                    var CTL = new ConverterList(_Name);
                    converterLists.Add(CTL);
                    _AddConverterProgress CP = new _AddConverterProgress(CTL.ConverterListAdd);
                    return CP;
                }
                return null;
            }
            catch(Exception ex)
            {
                ex.LogWrite($"AddNewProgressEventHandler {MethodBase.GetCurrentMethod().Name}");
                return null;
            }
        }

        public void SetScrollHander(string _Name, OnNotifyInsert scrollIntoViewHandler)
        {
            try
            {
                foreach (var v in converterLists.Where(v => v.DestName == _Name))
                {
                    v.DoOnInsert = scrollIntoViewHandler;
                    break;
                }
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddNewProgressEventHandler {MethodBase.GetCurrentMethod().Name}");
            }
        }
    }

    public class ConverterList
    {
        public string DestName;
        public OnNotifyInsert DoOnInsert;
        public ObservableCollection<string> ProgressList = new ObservableCollection<string>();
        public ConverterList(string _Name)
        {
            try
            {
                DestName = _Name;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CoverterList.Create {MethodBase.GetCurrentMethod().Name}");
            }
        }

        public string DestNameNoExt()
        {
            try
            {
                return System.IO.Path.GetFileNameWithoutExtension(DestName);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"CoverterList.Create {MethodBase.GetCurrentMethod().Name}");
                return "";
            }
        }

        public void ConverterListAdd(string _name,string _data)
        {
            try
            {
                ProgressList.Add(_data);
                DoOnInsert?.Invoke(ProgressList.Count-1);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ConverterListAdd {MethodBase.GetCurrentMethod().Name}");
            }
        }
    }
   
}
