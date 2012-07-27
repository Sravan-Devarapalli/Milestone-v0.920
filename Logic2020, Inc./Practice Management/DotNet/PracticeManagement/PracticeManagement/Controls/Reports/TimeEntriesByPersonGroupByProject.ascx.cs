using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataTransferObjects.CompositeObjects;
using DataTransferObjects.TimeEntry;
using PraticeManagement.CalendarService;
using DataTransferObjects;

namespace PraticeManagement.Controls.Reports
{
    public partial class TimeEntriesByPersonGroupByProject : System.Web.UI.UserControl
    {
        #region Fields

        protected DateTime CurrDate;
        protected double GrandTotal;
        protected double ProjectTotals;
        protected int ColspanForTotals;
        private int calendarPersonId;


        #endregion

        public DateTime? StartDate
        {
            get;
            set;
        }


        public DateTime? EndDate
        {
            get;
            set;
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }


        private static IEnumerable<KeyValuePair<DateTime, double>> GetTotalsByDate(Dictionary<string, TimeEntryRecord[]> groupedTimeEtnries)
        {
            var res = new SortedDictionary<DateTime, double>();

            foreach (var etnry in groupedTimeEtnries)
                foreach (var record in etnry.Value)
                {
                    var date = record.ChargeCodeDate;
                    var hours = record.TotalHours;

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
                    var dic = new Dictionary<string, TimeEntryRecord[]>();

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

                        var startDate = StartDate.HasValue ? StartDate.Value.Date : DateTime.Now.Date;
                        var endDate = EndDate.HasValue ? EndDate.Value.Date : DateTime.Now.Date;

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

                if (StartDate.HasValue)
                {
                    var personId = calendarPersonId;
                    var startDate = StartDate.Value;
                    var endDate = EndDate.HasValue ? EndDate.Value : DateTime.Today;

                    using (var serviceClient = new CalendarServiceClient())
                    {
                        var result = serviceClient.GetPersonCalendar(startDate, endDate, personId, null);
                        dlProject.DataSource = result;
                        dlProject.DataBind();
                    }
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
                return timeEntries.Sum(item => item.TotalHours);
            }
            catch (Exception)
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
                ProjectTotals += ((KeyValuePair<DateTime, TimeEntryRecord>)(e.Item.DataItem)).Value.TotalHours;
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


        public void PopulateControls(PersonTimeEntries personTimeEnryDetails)
        {

            if (personTimeEnryDetails.Person != null)
            {
                calendarPersonId = personTimeEnryDetails.Person.Id.Value;
                divPersonName.InnerText = personTimeEnryDetails.Person.Name;
            }

            Dictionary<string, List<TimeEntryRecord>> list = new Dictionary<string, List<TimeEntryRecord>>();

            if (personTimeEnryDetails.GroupedTimeEtnries == null || personTimeEnryDetails.GroupedTimeEtnries.Count == 0)
            {
                lblnoDataMesssage.Visible = true;
                divProjects.Visible = false;
                divTeTable.Visible = false;
            }
            else
            {
                string seperator = " - ";

                list = personTimeEnryDetails.GroupedTimeEtnries.GroupBy(item => new
                {
                    ProjectId = item.Key.Project.Id,
                    GroupId = item.Key.ProjectGroup.Id,
                    ClientId = item.Key.Client.Id,
                    DisplayName = item.Key.Client.HtmlEncodedName +
                                  seperator +
                                  item.Key.ProjectGroup.HtmlEncodedName +
                                  seperator +
                                  item.Key.Project.ProjectNumber +
                                  seperator +
                                  item.Key.Project.HtmlEncodedName 
                })
                  .Select(group =>
                        new
                        {
                            Name = group.Key.DisplayName,
                            TERecords = personTimeEnryDetails.GroupedTimeEtnries.Where(t => t.Key.Project.Id == group.Key.ProjectId &&
                                                                                            t.Key.ProjectGroup.Id == group.Key.GroupId &&
                                                                                            t.Key.Client.Id == group.Key.ClientId 
                                                                                            ).SelectMany(k => k.Value.Select(t => t)).ToList()
                        })
                  .ToDictionary(k => k.Name, v => v.TERecords);

            }



            repTeTable.DataSource = list;
            repTeTable.DataBind();

            dlProjects.DataSource = list;
            dlProjects.DataBind();
        }

        protected Dictionary<DateTime, TimeEntryRecord> GetUpdatedDatasource(object teRecords)
        {
            List<TimeEntryRecord> teRecordsList = teRecords as List<TimeEntryRecord>;
            var listOfRecordsWithDates = new Dictionary<DateTime, TimeEntryRecord>();
            var startDate = StartDate.HasValue ? StartDate.Value.Date : DateTime.Now.Date;
            var endDate = EndDate.HasValue ? EndDate.Value.Date : DateTime.Now.Date;

            while (startDate <= endDate)
            {
                var ters = teRecordsList.Any(t => t.ChargeCodeDate.Date == startDate) ? teRecordsList.Where(t => t.ChargeCodeDate.Date == startDate) : null;
                TimeEntryRecord ter = null;

                if (ters != null)
                {
                    ter = new TimeEntryRecord()
                    {
                        BillableHours = ters.Sum(p => p.BillableHours),
                        NonBillableHours = ters.Sum(p => p.NonBillableHours)
                    };
                }

                listOfRecordsWithDates.Add(startDate, ter);
                startDate = startDate.AddDays(1);
            }

            return listOfRecordsWithDates;

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
    }
}
