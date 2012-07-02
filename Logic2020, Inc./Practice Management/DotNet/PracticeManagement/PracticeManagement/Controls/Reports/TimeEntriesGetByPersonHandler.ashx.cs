using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using DataTransferObjects.CompositeObjects;
using System.Text;

namespace PraticeManagement.Controls.Reports
{
    /// <summary>
    /// Summary description for TimeEntriesGetByPersonHandler
    /// </summary>
    public class TimeEntriesGetByPersonHandler : IHttpHandler
    {
        private const string ExcelDateFormat = "mso-number-format";
        private const string ExcelDateFormatStyle = @"MM\/dd\/yy";

        public void ProcessRequest(HttpContext context)
        {

            var exportToExcel = Convert.ToBoolean(context.Request.QueryString["ExportToExcel"]);

            var startDate = Convert.ToDateTime(context.Request.QueryString["StartDate"]);
            var endDate = Convert.ToDateTime(context.Request.QueryString["EndDate"]);
            var payTypeIds = context.Request.QueryString["PayScaleIds"];
            var practiceIds = context.Request.QueryString["PracticeIds"];

            int view = Convert.ToInt32(context.Request.QueryString["view"]);

            var practicesArray = practiceIds.Split(',');
            List<int> practices = new List<int>();
            foreach (var str in practicesArray)
            {
                int val;
                if (int.TryParse(str, out val))
                    practices.Add(val);
            }

            var payscaleArrays = payTypeIds.Split(',');
            List<int> payscales = new List<int>();
            foreach (var str in payscaleArrays)
            {
                int val;
                if (int.TryParse(str, out val))
                    payscales.Add(val);
            }

            if (exportToExcel)
            {
                DateIndex = -1;
                HoursIndex = -1;

                var personsArray = context.Request.QueryString["PersonID"].Split(',');
                List<int> persons = new List<int>();
                foreach (var str in personsArray)
                {
                    int val;
                    if (int.TryParse(str, out val))
                        persons.Add(val);
                }

                var reportContext = new DataTransferObjects.ContextObjects.TimeEntryPersonReportContext
                {
                    PersonIds = persons,
                    StartDate = startDate,
                    EndDate = endDate,
                    PayTypeIds = payTypeIds.ToLower() != "null" ? payscales : null,
                    PracticeIds = practiceIds.ToLower() != "null" ? practices : null
                };

                var dsPersons = ServiceCallers.Custom.TimeEntry(client => client.TimeEntriesByPersonGetExcelSet(reportContext));

                System.Web.UI.WebControls.GridView excelGrid = new System.Web.UI.WebControls.GridView();
                excelGrid.DataSource = dsPersons;
                excelGrid.RowDataBound += new GridViewRowEventHandler(excelGrid_RowDataBound);
                excelGrid.DataBind();

                Table summaryTable = new Table();
                TableRow tableRow0 = new TableRow();
                TableCell col0 = new TableCell();
                col0.Text = "Time Report by Person";
                col0.Font.Size = FontUnit.Point(18);
                col0.Font.Bold = true;
                tableRow0.Controls.Add(col0);
                summaryTable.Controls.Add(tableRow0);

                TableRow tableRow = new TableRow();
                TableCell col1 = new TableCell();
                TableCell col2 = new TableCell();
                TableCell col3 = new TableCell();

                col1.Text = "Reporting Period :  ";
                col1.Font.Bold = true;
                col1.Font.Size = FontUnit.Point(18);

                col2.Text = "<b>From: </b>" + reportContext.StartDate.Value.ToString("MM/dd/yyyy");
                col2.Font.Size = FontUnit.Point(18);
                col3.Text = "<b>To: </b>" + reportContext.EndDate.Value.ToString("MM/dd/yyyy");
                col3.Font.Size = FontUnit.Point(18);

                tableRow.Controls.Add(col1);
                tableRow.Controls.Add(col2);
                tableRow.Controls.Add(col3);

                summaryTable.Controls.Add(tableRow);
                TableRow tableRow2 = new TableRow();
                TableCell col21 = new TableCell();
                TableCell col22 = new TableCell();
                col21.Text = "Date Run: ";
                col21.Font.Size = FontUnit.Point(18);
                col21.Font.Bold = true;
                col22.Text = PraticeManagement.Utils.SettingsHelper.GetCurrentPMTime().ToString();
                col22.Style.Add(ExcelDateFormat, @"yyyy/MM/dd hh:mm \ AM\/PM");
                col22.Font.Size = FontUnit.Point(18);
                tableRow2.Controls.Add(col21);
                tableRow2.Controls.Add(col22);
                summaryTable.Controls.Add(tableRow2);

                GridViewExportUtil.Export("TimeEntry_Report_By_Person.xls", excelGrid, summaryTable, null, null);

            }
            else
            {
                var personid = Convert.ToInt32(context.Request.QueryString["PersonID"]);
            
                var personWithTimeEntry = PraticeManagement.Utils.TimeEntryHelper.GetTimeEntriesForPerson(personid, startDate, endDate, payTypeIds.ToLower() != "null" ? payscales : null, practiceIds.ToLower() != "null" ? practices : null);
                StringBuilder sb = new StringBuilder();

                if (personWithTimeEntry.Person != null)
                {


                    PraticeManagement.Sandbox.TimeEntriesByPerson page = new PraticeManagement.Sandbox.TimeEntriesByPerson();


                    if (view == 1)
                    {
                        TimeEntriesByPersonGroupByProject cntrlTimeEntriesByPerson = (TimeEntriesByPersonGroupByProject)page.LoadControl("~/Controls/Reports/TimeEntriesByPersonGroupByProject.ascx");

                        cntrlTimeEntriesByPerson.StartDate = startDate;
                        cntrlTimeEntriesByPerson.EndDate = endDate;
                        cntrlTimeEntriesByPerson.PopulateControls(personWithTimeEntry);

                        page.Controls.Add(cntrlTimeEntriesByPerson);

                        using (System.IO.StringWriter sw = new System.IO.StringWriter())
                        {
                            cntrlTimeEntriesByPerson.RenderControl(new HtmlTextWriter(sw));
                            sb.Append(sw);
                        }
                    }
                    else
                    {
                        TimeEntriesByPerson cntrlTimeEntriesByPerson = (TimeEntriesByPerson)page.LoadControl("~/Controls/Reports/TimeEntriesByPerson.ascx");

                        cntrlTimeEntriesByPerson.StartDate = startDate;
                        cntrlTimeEntriesByPerson.EndDate = endDate;
                        cntrlTimeEntriesByPerson.PopulateControls(personWithTimeEntry);

                        page.Controls.Add(cntrlTimeEntriesByPerson);

                        using (System.IO.StringWriter sw = new System.IO.StringWriter())
                        {
                            cntrlTimeEntriesByPerson.RenderControl(new HtmlTextWriter(sw));
                            sb.Append(sw);
                        }
                    }

                }

                var html = sb.ToString();
                RemoveWhitespaceFromHtml(html);
                context.Response.Write(html);
            }
        }


        private static readonly Regex RegexBetweenTags = new Regex(@">(?! )\s+", RegexOptions.Compiled);

        private static readonly Regex RegexLineBreaks = new Regex(@"([\n\s])+?(?<= {2,})<", RegexOptions.Compiled);


        public static void RemoveWhitespaceFromHtml(string html)
        {
            html = RegexBetweenTags.Replace(html, ">");
            html = RegexLineBreaks.Replace(html, "<").Trim();
        }


        void excelGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    var cell = e.Row.Cells[i];
                    if (cell.Text.ToLower() == "date")
                    {
                        DateIndex = i;
                    }
                    if (cell.Text.ToLower() == "hours")
                    {
                        HoursIndex = i;
                    }

                    cell.HorizontalAlign = HorizontalAlign.Left;
                }

            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (DateIndex != -1)
                {
                    var cell = e.Row.Cells[DateIndex];
                    cell.Style.Add(ExcelDateFormat, ExcelDateFormatStyle);
                }

                if (HoursIndex != -1)
                {
                    var cell = e.Row.Cells[HoursIndex];
                    cell.HorizontalAlign = HorizontalAlign.Center;
                }

                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.VerticalAlign = VerticalAlign.Top;
                }
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private int dateIndex;

        public int DateIndex
        {
            get
            {
                return dateIndex;
            }
            set
            {
                dateIndex = value;
            }
        }

        private int hoursIndex;

        public int HoursIndex
        {
            get
            {
                return hoursIndex;
            }
            set
            {
                hoursIndex = value;
            }
        }


    }
}

