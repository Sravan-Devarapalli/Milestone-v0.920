using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using DataTransferObjects.Utils;
using PraticeManagement.Controls;
using PraticeManagement.Controls.Persons;
using PraticeManagement.Controls.TimeEntry;
using DataTransferObjects.CompositeObjects;
using System.Linq;
using System.Web.Security;
using DataTransferObjects;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web;
using PraticeManagement.Objects;

using PraticeManagement.CalendarService;

namespace PraticeManagement.Sandbox
{
    public partial class TimeEntriesByPerson : PracticeManagementPageBase
    {
        #region Fields

        protected DateTime CurrDate;
        protected double GrandTotal;
        protected double ProjectTotals;
        protected int ColspanForTotals;
        private int calendarPersonId;

        #endregion

        #region Constants

        private const string TEByPersonExport = "Time Entry By Person";

        #endregion

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            if (diRange.FromDate.HasValue)
            {
                btnExportToXL.Disabled = false;
                btnExportToPDF.Enabled = true;

                var personIds = cblPersons.SelectedValues;

                personIds = personIds.Count > 0 ? new List<int>() {  } : new List<int>();
                var startDate = this.diRange.FromDate.Value;
                var endDate = this.diRange.ToDate.HasValue ? this.diRange.ToDate.Value : DateTime.Today;
                var payTypeIds = this.cblTimeScales.SelectedValues;
                var practiceIds = this.cblPractices.SelectedValues;
                //var persons = PraticeManagement.Utils.TimeEntryHelper.GetTimeEntriesForPerson(personIds, startDate, endDate, payTypeIds, practiceIds);
                //repPersons.DataSource = persons;
                //repPersons.DataBind();
                System.Web.UI.ScriptManager.RegisterClientScriptBlock(updReport, updReport.GetType(), "", "SetDivWidth();", true);
                if (hdnFiltersChanged.Value == "false")
                {
                    btnResetFilter.Attributes.Add("disabled", "true");
                }
                else
                {
                    btnResetFilter.Attributes.Remove("disabled");
                }
                AddAttributesToCheckBoxes(this.cblPractices);
                AddAttributesToCheckBoxes(this.cblTimeScales);
                AddAttributesToCheckBoxes(this.cblPersons);

                var selectedvalues = cblPersons.SelectedItems;
                var index = selectedvalues.Any(c => c == ',') ? selectedvalues.IndexOf(',') : -1;
                hdnPersonIds.Value = cblPersons.SelectedItems;// (index != -1) ? selectedvalues.Remove(0, index + 1) : "";

                hdnPracticeIds.Value = practiceIds != null ? cblPractices.SelectedItems : "null";
                hdnPayScaleIds.Value = payTypeIds != null ? cblTimeScales.SelectedItems : "null";
                hdnStartDate.Value = startDate.ToString();
                hdnEndDate.Value = endDate.ToString();
                hlnkExportToExcel.NavigateUrl = "../Controls/Reports/TimeEntriesGetByPersonHandler.ashx?ExportToExcel=true&PersonID="
                    + cblPersons.SelectedItems + "&StartDate=" + startDate.ToString() + "&EndDate=" + endDate.ToString()
                    + "&PayScaleIds=" + (payTypeIds != null ? cblTimeScales.SelectedItems : "null") + "&PracticeIds=" + (practiceIds != null ? cblPractices.SelectedItems : "null");

                hdnUpdateClicked.Value = "true";
                updReport.Update();
            }
        }

        protected void ExportToPDF(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TEByPersonExport);

            string fileName = "TimeEntriesForPerson.pdf";
            var html = hdnSaveReportText.Value;
            HTMLToPdf(html, fileName);
        }

        public void HTMLToPdf(String HTML, string fileName)
        {
            if (HTML == String.Empty)
            {
                HTML = " &nbsp;  ";
            }

            HtmlToPdfBuilder builder = new HtmlToPdfBuilder(iTextSharp.text.PageSize.A4_LANDSCAPE);

            string[] splitArray = { hdnGuid.Value };

            string[] htmlArray = HTML.Split(splitArray, StringSplitOptions.RemoveEmptyEntries);

            foreach (var html in htmlArray)
            {
                HtmlPdfPage page = builder.AddPage();

                page.AppendHtml("<div>{0}</div>", html);

            }

            byte[] timeEntriesByPersonBytes = builder.RenderPdf();

            HttpContext.Current.Response.ContentType = "Application/pdf";
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));


            int len = timeEntriesByPersonBytes.Length;
            int bytes;
            byte[] buffer = new byte[1024];

            Stream outStream = HttpContext.Current.Response.OutputStream;
            using (MemoryStream stream = new MemoryStream(timeEntriesByPersonBytes))
            {
                while (len > 0 && (bytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outStream.Write(buffer, 0, bytes);
                    HttpContext.Current.Response.Flush();
                    len -= bytes;
                }
            }


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hdnUpdateClicked.Value = "false";

            TextBox fromDate = diRange.FindControl("tbFrom") as TextBox;
            TextBox toDate = diRange.FindControl("tbTo") as TextBox;

            if (!IsPostBack)
            {
                hdnGuid.Value = Guid.NewGuid().ToString();
                Utils.Generic.InitStartEndDate(diRange);
                Populatepersons(true);
                DataHelper.FillPracticeList(this.cblPractices, Resources.Controls.AllPracticesText);
                DataHelper.FillTimescaleList(this.cblTimeScales, Resources.Controls.AllTypes);
                SelectAllItems(this.cblPractices);
                SelectAllItems(this.cblTimeScales);
                if (hdnFiltersChanged.Value == "false")
                {
                    btnResetFilter.Attributes.Add("disabled", "true");
                }
                else
                {
                    btnResetFilter.Attributes.Remove("disabled");
                }
                AddAttributesToCheckBoxes(this.cblPractices);
                AddAttributesToCheckBoxes(this.cblTimeScales);
                AjaxControlToolkit.CalendarExtender fromDateExt = diRange.FindControl("clFromDate") as AjaxControlToolkit.CalendarExtender;
                AjaxControlToolkit.CalendarExtender toDateExt = diRange.FindControl("clToDate") as AjaxControlToolkit.CalendarExtender;



                fromDate.AutoPostBack = true;
                toDate.AutoPostBack = true;
                fromDate.CausesValidation = true;
                toDate.CausesValidation = true;
                fromDateExt.OnClientDateSelectionChanged = "EnableResetButton";
                toDateExt.OnClientDateSelectionChanged = "EnableResetButton";
            }
            fromDate.Attributes.Add("onchange", "return CheckIsPostBackRequired(this);");
            toDate.Attributes.Add("onchange", "return CheckIsPostBackRequired(this);");
            fromDate.TextChanged += diRange_SelectionChanged;
            toDate.TextChanged += diRange_SelectionChanged;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdatePanel1.Update();
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Attributes.Add("onclick", "EnableResetButton();");
            }
        }

        private string GetStatusIds()
        {
            string statusList = string.Empty;
            if (chbActivePersons.Checked)
                statusList += ((int)PersonStatusType.Active).ToString();
            if (chbInactivePersons.Checked)
                statusList += (string.IsNullOrEmpty(statusList) ? string.Empty : ",") + ((int)PersonStatusType.Inactive).ToString();
            if (chbTerminatedPersons.Checked)
                statusList += (string.IsNullOrEmpty(statusList) ? string.Empty : ",") + ((int)PersonStatusType.Terminated).ToString();
            if (string.IsNullOrEmpty(statusList))
                statusList = "-1";
            return statusList;
        }

        protected override void Display()
        {
            //Utils.Generic.InitStartEndDate(diRange);
        }

        protected void wsChoose_WeekChanged(object sender, WeekChangedEventArgs args)
        {
        }

        protected void pcPersons_PersonChanged(object sender, PersonChangedEventArguments args)
        {
        }

        private static IEnumerable<KeyValuePair<DateTime, double>> GetTotalsByDate<T>(Dictionary<T, TimeEntryRecord[]> groupedTimeEtnries)
        {
            var res = new SortedDictionary<DateTime, double>();

            foreach (var etnry in groupedTimeEtnries)
                foreach (var record in etnry.Value)
                {
                    var date = record.MilestoneDate;
                    var hours = record.ActualHours;

                    try
                    {
                        res[date] += hours;
                    }
                    catch (Exception)
                    {
                        res.Add(date, hours);
                    }
                }

            return res;
        }

        protected void repTeTable_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Footer)
            {
                var dsource = ((sender as Repeater).DataSource as Dictionary<string, List<TimeEntryRecord>>);
                if (dsource != null)
                {
                    Dictionary<string, TimeEntryRecord[]> dic = new Dictionary<string, TimeEntryRecord[]>();

                    foreach (var item in dsource)
                    {
                        dic.Add(item.Key, item.Value.ToArray());
                    }


                    var totalsFooter = e.Item.FindControl("dlTotals") as Repeater;
                    if (totalsFooter != null)
                    {
                        var totals = GetTotalsByDate(dic).ToList();

                        var modifiedTotals = new List<KeyValuePair<DateTime, double?>>();

                        foreach (var item in totals)
                        {
                            modifiedTotals.Add(new KeyValuePair<DateTime, double?>(item.Key, item.Value));
                        }

                        var startDate = diRange.FromDate.HasValue ? diRange.FromDate.Value.Date : DateTime.Now.Date;
                        var endDate = diRange.ToDate.HasValue ? diRange.ToDate.Value.Date : DateTime.Now.Date;

                        while (startDate <= endDate)
                        {
                            if (!totals.Any(t => t.Key.Date == startDate))
                            {
                                modifiedTotals.Add(new KeyValuePair<DateTime, double?>(startDate, null));
                            }

                            startDate = startDate.AddDays(1);
                        }

                        var sortedDict = (from entry in modifiedTotals
                                          orderby entry.Key ascending
                                          select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

                        totalsFooter.DataSource = sortedDict;
                        totalsFooter.DataBind();
                    }
                }
            }
        }


        protected void repTeTable_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var dlProject = e.Item.FindControl("dlProject") as Repeater;

                if (diRange.FromDate.HasValue)
                {
                    var personId = calendarPersonId;
                    var startDate = this.diRange.FromDate.Value;
                    var endDate = this.diRange.ToDate.HasValue ? this.diRange.ToDate.Value : DateTime.Today;

                    using (var serviceClient = new CalendarServiceClient())
                    {
                        var result = serviceClient.GetCalendar(startDate, endDate, personId, null);
                        dlProject.DataSource = result;
                        dlProject.DataBind();
                    }
                }
            }
        }

        protected void dlPersons_OnItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem != null)
                {
                    var item = (TimeEntriesGroupedByPersonProject)e.Item.DataItem;
                    calendarPersonId = item.PersonId;
                }
            }
        }

        protected void gvTimeEntries_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.Footer)
            {
                e.Row.Cells[0].Attributes["valign"] = "middle";
                e.Row.Cells[1].Attributes["valign"] = "middle";
                e.Row.Cells[2].Attributes["valign"] = "middle";
                e.Row.Cells[3].Attributes["valign"] = "middle";

                if (e.Row.RowType == DataControlRowType.Footer)
                {
                    foreach (TableCell cell in e.Row.Cells)
                    {
                        cell.BorderStyle = BorderStyle.None;
                    }
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                var dsource = (sender as GridView).DataSource as List<TimeEntryRecord>;
                if (dsource != null)
                {
                    var tatalLabel = e.Row.FindControl("lblGvGridTotal") as Label;
                    tatalLabel.Text = GetTotalActualHours(dsource).ToString(PraticeManagement.Constants.Formatting.DoubleFormat);
                }
            }
        }

        private double GetTotalActualHours(List<TimeEntryRecord> timeEntries)
        {
            try
            {
                return timeEntries.Sum(item => item.ActualHours);
            }
            catch (Exception ex)
            {
                return 0.00;
            }
        }


        protected void dlTotals_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex == 0)
                GrandTotal = 0;

            if (e.Item.DataItem is KeyValuePair<DateTime, double?> && ((KeyValuePair<DateTime, double?>)e.Item.DataItem).Value != null)
                GrandTotal += ((KeyValuePair<DateTime, double?>)e.Item.DataItem).Value.Value;
        }

        protected void dlProject_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemIndex == 0)
                ProjectTotals = 0;

            if (e.Item.DataItem is KeyValuePair<DateTime, TimeEntryRecord> && ((KeyValuePair<DateTime, TimeEntryRecord>)(e.Item.DataItem)).Value != null)
                ProjectTotals += ((KeyValuePair<DateTime, TimeEntryRecord>)(e.Item.DataItem)).Value.ActualHours;
        }

        protected void dlTotals_OnInit(object sender, EventArgs e)
        {
            GrandTotal = 0;
        }

        protected void dlProject_OnItemCreated(object sender, EventArgs e)
        {
            ColspanForTotals += 1;
        }

        protected void dlProject_OnInit(object sender, EventArgs e)
        {
            ColspanForTotals = 0;
        }

        protected void odsCalendar_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {

        }


        protected void btnResetFilter_OnClick(object sender, EventArgs e)
        {
            Utils.Generic.InitStartEndDate(diRange);
            this.chbActivePersons.Checked = true;
            this.chbInactivePersons.Checked = this.chbTerminatedPersons.Checked = false;
            SelectAllItems(this.cblPractices);
            SelectAllItems(this.cblTimeScales);
            Populatepersons(false);
            hdnFiltersChanged.Value = "false";
            btnResetFilter.Attributes.Add("disabled", "true");
            AddAttributesToCheckBoxes(this.cblPractices);
            AddAttributesToCheckBoxes(this.cblTimeScales);
            SelectCurrentPerson(DataHelper.CurrentPerson.Id);
        }

        private void SelectAllItems(ScrollingDropDown ddlpractices)
        {
            foreach (ListItem item in ddlpractices.Items)
            {
                item.Selected = true;
            }
        }

        protected void PersonStatus_OnCheckedChanged(object sender, EventArgs e)
        {
            var currentlySelectedPersons = this.cblPersons.SelectedValues;

            Populatepersons(false);
            this.cblPersons.DataSource = null;

            foreach (var selectedPersonId in currentlySelectedPersons)
            {

                ListItem item = this.cblPersons.Items.FindByValue(selectedPersonId.ToString());
                if (item != null)
                    item.Selected = true;
            }
            this.cblPersons.Items[0].Selected = false;
            if (hdnFiltersChanged.Value == "false")
            {
                btnResetFilter.Attributes.Add("disabled", "true");
            }
            else
            {
                btnResetFilter.Attributes.Remove("disabled");
            }
        }

        protected void diRange_SelectionChanged(object sender, EventArgs e)
        {
            if (diRange.FromDate <= diRange.ToDate)
            {
                PersonStatus_OnCheckedChanged(sender, e);
            }
        }

        private void Populatepersons(bool enableDisableChevron)
        {
            var currentPerson = DataHelper.CurrentPerson;
            var personRoles = Roles.GetRolesForUser(currentPerson.Alias);
            string statusIdsList = GetStatusIds();
            int? personId = null;
            if (!personRoles.Any(s => s == "Administrator" || s == "HR"))
            {
                personId = currentPerson.Id;
                if (enableDisableChevron)
                {
                    this.cpe.Enabled = false;
                    pnlFilters.Visible = false;
                }
            }
            DataHelper.FillTimeEntryPersonListBetweenStartAndEndDates(this.cblPersons, Resources.Controls.AllPersons, null, statusIdsList, personId, diRange.FromDate, diRange.ToDate);
            AddAttributesToCheckBoxes(this.cblPersons);
        }

        protected void Page_SaveStateComplete(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SelectCurrentPerson(DataHelper.CurrentPerson.Id);
            }
        }

        private void SelectCurrentPerson(int? personId)
        {
            if (personId.HasValue)
            {
                var currentPersonItem = this.cblPersons.Items.FindByValue(personId.ToString());
                if (currentPersonItem != null)
                {
                    currentPersonItem.Selected = true;

                }
            }
        }

        protected void dlPersons_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            var dataItem = (TimeEntriesGroupedByPersonProject)e.Item.DataItem;
            if (dataItem.GroupedTimeEtnries == null || dataItem.GroupedTimeEtnries.Count == 0)
            {
                var divProjects = e.Item.FindControl("divProjects") as System.Web.UI.HtmlControls.HtmlGenericControl;
                var divTeTable = e.Item.FindControl("divTeTable") as System.Web.UI.HtmlControls.HtmlGenericControl;
                var lblnoDataMesssage = e.Item.FindControl("lblnoDataMesssage") as Label;
                lblnoDataMesssage.Visible = true;
                divProjects.Visible = false;
                divTeTable.Visible = false;
            }
        }

        protected Dictionary<DateTime, TimeEntryRecord> GetUpdatedDatasource(object teRecords)
        {
            List<TimeEntryRecord> teRecordsList = teRecords as List<TimeEntryRecord>;
            var listOfRecordsWithDates = new Dictionary<DateTime, TimeEntryRecord>();
            var startDate = diRange.FromDate.HasValue ? diRange.FromDate.Value.Date : DateTime.Now.Date;
            var endDate = diRange.ToDate.HasValue ? diRange.ToDate.Value.Date : DateTime.Now.Date;

            while (startDate <= endDate)
            {
                var ters = teRecordsList.Any(t => t.MilestoneDate.Date == startDate) ? teRecordsList.Where(t => t.MilestoneDate.Date == startDate) : null;
                TimeEntryRecord ter = null;

                if (ters != null)
                {
                    ter = new TimeEntryRecord()
                    {
                        ActualHours = ters.Sum(p => p.ActualHours)
                    };
                }

                listOfRecordsWithDates.Add(startDate, ter);
                startDate = startDate.AddDays(1);
            }

            return listOfRecordsWithDates;

        }


        protected Dictionary<string, List<TimeEntryRecord>> GetModifiedDatasource(object groupedTES)
        {
            Dictionary<Project, List<TimeEntryRecord>> groupedTESList = groupedTES as Dictionary<Project, List<TimeEntryRecord>>;


            var modifiedgroupedTESList = new Dictionary<string, List<TimeEntryRecord>>();

            if (groupedTESList != null && groupedTESList.Count > 0)
            {

                foreach (var keyVal in groupedTESList)
                {
                    if (keyVal.Value.Count() > 0)
                    {
                        var timeTypes = keyVal.Value.Select(t => t.TimeType.Name).Distinct();

                        foreach (var name in timeTypes)
                        {
                            modifiedgroupedTESList.Add(keyVal.Key.Client.Name + " - " + keyVal.Key.Name
                           + " - " + name, keyVal.Value.Where(k => k.TimeType.Name == name).ToList());
                        }

                    }
                }

            }


            return modifiedgroupedTESList;
        }

        protected void dlProjects_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var gv = e.Item.FindControl("gvTimeEntries") as GridView;
                if (gv != null && gv.Rows.Count == 0)
                {
                    gv.GridLines = GridLines.None;
                }

            }

        }

        public override void VerifyRenderingInServerForm(Control control)
        {

            /* Verifies that the control is rendered */

        }


    }
}

