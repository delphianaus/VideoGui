using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VideoGui.Models.delegates;

namespace VideoGui
{
    public class WebAddressBuilder
    {
        public string Address = "", channel_ID = "";
        public AddressUpdate DoOnAddressUpdate = null;
        public AddressUpdateId DoOnExtWebAddressUpdate = null;
        List<string> FilterByStrings = new List<string>();
        string SortBy = "", ExtWebAddress = "", Id = "";
        public bool IsShorts = false;

        public string ScopeAddress => ExtWebAddress;
        public string ChannelID => Address;
        public WebAddressBuilder(AddressUpdate doOnAddressUpdate, AddressUpdateId doOnExtAddressUpdate, string Channel_ID)
        {
            try
            {
                channel_ID = Channel_ID;
                DoOnAddressUpdate = doOnAddressUpdate;
                DoOnExtWebAddressUpdate = doOnExtAddressUpdate;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"WebAddressBuilder Constructor {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
            }
        }
        public WebAddressBuilder(string Channel_ID)
        {
            try
            {
                channel_ID = Channel_ID;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"WebAddressBuilder Constructor {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
            }
        }
        public WebAddressBuilder Start()
        {
            try
            {
                if (FilterByStrings is not null)
                {
                    FilterByStrings.Clear();
                }
                SortBy = "";
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Start {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
                return this;
            }
        }

        public WebAddressBuilder AddFilterByShorts()
        {
            try
            {
                IsShorts = true;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByShorts {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
                return this;
            }
        }

        public void FinalizeExt()
        {
            try
            {
                DoOnExtWebAddressUpdate?.Invoke(ExtWebAddress, Id);
            }
            catch (Exception ex)
            {
                ex.LogWrite($"FinalizeExt {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
                //return this;
            }
        }

        public WebAddressBuilder GetChannelURL()
        {
            try
            {
                Address = $"https://www.youtube.com/channel/{channel_ID}";
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"GetChannelURL {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder ScopeVideo(string videoId, bool dFinalize = false)
        {
            try
            {
                ExtWebAddress = $"https://studio.youtube.com/video/{videoId}/edit";
                Id = videoId;
                if (dFinalize) FinalizeExt();
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScopeVideo {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
                return this;
            }
        }

        public WebAddressBuilder ScopeVideoAddress(string videoId, bool dFinalize = false)
        {
            try
            {
                ExtWebAddress = videoId;
                Id = "";
                if (dFinalize) FinalizeExt();
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"ScopeVideo {MethodBase.GetCurrentMethod().Name} {this} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder SortyByViews(SortOrder order, bool DoFinalize = false)
        {
            try
            {
                string p = (order == SortOrder.ASCENDING) ? "ASCENDING" : "DESCENDING";
                SortBy = @"{""columnType"":""views"",""sortOrder"":""@0""}".Replace("@0", p);
                if (DoFinalize)
                {
                    Finalize();
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SortyByViews {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder SortByDateDESC(bool DoFinalize = false)
        {
            try
            {
                var p = SortByDate(SortOrder.DESCENDING);
                if (DoFinalize)
                {
                    Finalize();
                }
                return p;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SortyByDateDesc {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder SortByDateASSEND(bool DoFinalize = false)
        {
            try
            {
                var p = SortByDate(SortOrder.ASCENDING);
                if (DoFinalize)
                {
                    Finalize();
                }
                return p;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SortyByDateASSEND {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder SortByDate(SortOrder order, bool DoFinalize = false)
        {
            try
            {
                string p = (order == SortOrder.ASCENDING) ? "ASCENDING" : "DESCENDING";
                SortBy = @"{""columnType"":""date"",""sortOrder"":""@0""}".Replace("@0", p);
                if (DoFinalize)
                {
                    Finalize();
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"SortyByDate {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterMadeForKids(List<MadeForKids> values, bool DoFinalize = false)
        {
            try
            {
                //if (value. MadeForKids.MFK_SET_BY_YOU) MFK.Add("MFK_SET_BY_YOU");
                List<string?> t = values.Select(kid =>
                {
                    switch (kid)
                    {
                        case MadeForKids.MFK_SET_BY_YOU:
                            return "\"MFK_SET_BY_YOU\"";
                        case MadeForKids.MFK_SET_BY_YOUTUBE:
                            return "\"MFK_SET_BY_YOUTUBE\"";
                        case MadeForKids.NOT_MADE_FOR_KIDS:
                            return "\"NOT_MADE_FOR_KIDS\"";
                        case MadeForKids.NO_SELECTION:
                            return "\"NO_SELECTION\"";
                        default:
                            return null;
                    }
                }).Where(x => x != null).ToList();

                string p = string.Join(",", t);
                if (p != "")
                {
                    string FilterBase = @"{""name"":""MFK_RESTRICTIONS"",""value"":[""@NO""]}]".Replace("@NO", p);
                    FilterByStrings.Add(HttpUtility.UrlEncode(FilterBase));
                }
                if (DoFinalize)
                {
                    Finalize();
                }
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByMadeForKids {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByDesc(string Desc, bool DoFinalze = false)
        {
            try
            {
                var p = AddFilterByTitleDesc(TitleDesc.DESCRIPTION, Desc);
                if (DoFinalze)
                {
                    Finalize();
                }
                return p;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByDesc {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByTitle(string title, bool DoFinialize = false)
        {
            try
            {
                var p = AddFilterByTitleDesc(TitleDesc.TITLE, title);
                if (DoFinialize)
                {
                    Finalize();
                }
                return p;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByTitle {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByTitleDesc(TitleDesc ftype, string Value, bool DoFinialize = false)
        {
            try
            {
                string Filter = (ftype == TitleDesc.TITLE) ? "TITLE" : "DESCRIPTION";
                string FilterBase = @"{""name"":""@1"",""value"":{""name"":""CONTAINS"",""value"":""@2""}}".Replace("@1", Filter).Replace("@2", Value);
                FilterByStrings.Add(FilterBase);
                if (DoFinialize)
                {
                    Finalize();
                }
                return this;

            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByDesc {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFiltersBySCHEDULED(bool _IsShorts = false)
        {
            try
            {
                AddFilterByVisibilityStatus(new List<StatusTypes> { StatusTypes.HAS_SCHEDULE }, false);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFiltersByDRAFT_UNLISTED {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFiltersByDRAFT_UNLISTED(bool _IsShorts = false)
        {
            try
            {
                AddFilterByVisibilityStatus(new List<StatusTypes> { StatusTypes.DRAFT, StatusTypes.UNLISTED }, false);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFiltersByDRAFT_UNLISTED {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }

        public WebAddressBuilder Dashboard()
        {
            try
            {
                Address = $"https://studio.youtube.com/channel/{channel_ID}";
                return this;
            }
            catch(Exception ex)
            {
                ex.LogWrite($"");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByDraftShorts(StatusTypes Video_status = StatusTypes.DRAFT, bool _IsShorts = true)
        {
            try
            {
                IsShorts = _IsShorts;
                if (Video_status == StatusTypes.DRAFT && !IsShorts)
                {
                    AddFilterByTitle("TEMPLATE", false);
                }
                AddFilterByVisibilityStatus(new List<StatusTypes> { Video_status }, false);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByDraftShorts {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }

        public WebAddressBuilder AddFilterByUnlistedShorts()
        {
            try
            {
                IsShorts = true;
                AddFilterByVisibilityStatus(new List<StatusTypes> { StatusTypes.UNLISTED }, true);
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByUnlistedShorts {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByCopyRightClaim(bool DoFinalize = false)
        {
            try
            {
                string FilterBase = @"{""name"":""HAS_COPYRIGHT_CLAIM"",""value"":""VIDEO_HAS_COPYRIGHT_CLAIM""}";
                FilterByStrings.Add(HttpUtility.UrlEncode(FilterBase));
                if (DoFinalize) Finalize();
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByCopyRightClaim {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }

        public string GetHTML()
        {
            try
            {
                string html = "";

                string StringFilter = $"[{string.Join(",", FilterByStrings)}]";
                if (SortBy == "") SortBy = @"{""columnType"":""date"",""sortOrder"":""DESCENDING""}";
                html = $"https://studio.youtube.com/channel/{channel_ID}/videos/" + (IsShorts ? "short" : "upload") + "?filter=" +
                    HttpUtility.UrlEncode(StringFilter) + "&sort=" + HttpUtility.UrlEncode(SortBy);
                return html;

            }
            catch (Exception ex)
            {
                ex.LogWrite($"gethtml {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return "";
            }
        }
        public WebAddressBuilder Finalize()
        {
            try
            {
                string html = "";

                string StringFilter = $"[{string.Join(",", FilterByStrings)}]";
                if (SortBy == "") SortBy = @"{""columnType"":""date"",""sortOrder"":""DESCENDING""}";
                html = $"https://studio.youtube.com/channel/{channel_ID}/videos/" + (IsShorts ? "short" : "upload") + "?filter=" +
                    HttpUtility.UrlEncode(StringFilter) + "&sort=" + HttpUtility.UrlEncode(SortBy);
                DoOnAddressUpdate?.Invoke(html);
                Address = html;
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"Finalize {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByAgeRestriction(AgeRestriction AgeViewValue, bool DoFinalize = false)
        {
            try
            {
                string Age = AgeViewValue == AgeRestriction.AGE_RESTRICTION ? "AGE_RESTRICTED" : "NOT_AGE_RESTRICTED";
                string basefilter = "{\"name\":\"AGE_RESTRICTION\",\"value\":\"@@\"}".Replace("@@", Age);//@"{""name"":""AGE_RESTRICTION"",""value"":"+Age+"}";
                FilterByStrings.Add(HttpUtility.UrlEncode(basefilter));
                if (DoFinalize) Finalize();
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByAgeRestriction {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByViews(ViewType sign, string ViewValue, bool DoFinalize = false)
        {
            try
            {
                string vt = (sign == ViewType.GREATER_EQUAL) ? "GREATER_EQUAL" : "LESS_EQUAL";
                string basefilter = @"{""name"":""VIEWS"",""value"":{""name"":""GREATER_EQUAL"",""value"":100}}".
                    Replace("GREATER_EQUAL", vt).Replace("100", ViewValue);
                FilterByStrings.Add(HttpUtility.UrlEncode(basefilter));
                if (DoFinalize) Finalize();
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByViews {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }

        public WebAddressBuilder AddFilterByPrivateStatus()
        {
            try
            {
                try
                {
                    AddFilterByVisibilityStatus(new List<StatusTypes> { StatusTypes.PRIVATE }, true);
                    return this;
                }
                catch (Exception ex)
                {
                    ex.LogWrite($"AddFilterByUnlistedShorts {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                    return this;
                }

            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByPrivateStatus {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
        public WebAddressBuilder AddFilterByVisibilityStatus(List<StatusTypes> status, bool DoFinalize = false)
        {
            try
            {
                FilterByStrings.Clear();
                List<string?> t = status.Select(stat =>
                {
                    switch (stat)
                    {
                        case StatusTypes.PUBLIC:
                            return "\"PUBLIC\"";
                        case StatusTypes.UNLISTED:
                            return "\"UNLISTED\"";
                        case StatusTypes.DRAFT:
                            return "\"DRAFT\"";
                        case StatusTypes.PRIVATE:
                            return "\"PRIVATE\"";
                        case StatusTypes.HAS_SCHEDULE:
                            return "\"HAS_SCHEDULE\"";

                        default:
                            return null;
                    }
                }).Where(x => x != null).ToList();

                string p = string.Join(",", t);

                if (p != "")
                {
                    string filter = @"{""name"":""VISIBILITY"",""value"":[@0]}".Replace("@0", p);
                    FilterByStrings.Add(filter);
                }
                if (DoFinalize) Finalize();
                return this;
            }
            catch (Exception ex)
            {
                ex.LogWrite($"AddFilterByStatus {MethodBase.GetCurrentMethod()?.Name} {ex.Message}");
                return this;
            }
        }
    }
}
