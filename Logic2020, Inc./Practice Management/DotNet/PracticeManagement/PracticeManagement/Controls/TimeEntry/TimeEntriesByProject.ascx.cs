using System;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using System.Collections.Generic;
using DataTransferObjects;
using System.Web.Security;
using System.Linq;
using PraticeManagement.Configuration;
using System.Web.UI;
using System.Web;
using System.IO;
using PraticeManagement.Objects;
using System.Text;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class TimeEntriesByProject : System.Web.UI.UserControl
    {
        #region constants

        private const int HoursCellIndex = 2;
        private double _totalPersonHours;
        private double _grandTotalHours;

        private const string TEByProjectExport = "Time Entry By Project";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            btnReset.Attributes["onclick"] = "return SelectAllPersons();";
            ResetTotalCounters();
            if (!IsPostBack)
            {
                hdnGuid.Value = Guid.NewGuid().ToString();

                //  If current user is administrator, don't apply restrictions
                Person person =
                    Roles.IsUserInRole(
                        DataHelper.CurrentPerson.Alias,
                        DataTransferObjects.Constants.RoleNames.AdministratorRoleName)
                        ? null
                        : DataHelper.CurrentPerson;
                var clients = DataHelper.GetAllClientsSecure(person, true);
                DataHelper.FillListDefault(ddlClients, "-- Select a Client -- ", clients as object[], false);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            AddAttributesToCheckBoxes(this.cblPersons);

            if (hdnResetFilter.Value == "false" || cblPersons.Items.Count == 0)
            {
                btnReset.Enabled = false;
            }
            else
            {
                btnReset.Enabled = true;
            }
        }

        private void AddAttributesToCheckBoxes(ScrollingDropDown ddl)
        {
            foreach (ListItem item in ddl.Items)
            {
                item.Attributes.Add("onclick", "EnableOrDisableResetFilterButton();");
            }
        }

        private void ResetTotalCounters()
        {
            _grandTotalHours = 0;
            _totalPersonHours = 0;
        }

        protected void gvPersonTimeEntries_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.Header:
                    _totalPersonHours = 0;
                    break;

                case DataControlRowType.DataRow:
                    _totalPersonHours += (e.Row.DataItem as TimeEntryRecord).ActualHours;
                    break;

                case DataControlRowType.Footer:
                    e.Row.Cells[HoursCellIndex].Text = "<div style='text-align: center;'>"+ _totalPersonHours.ToString(Constants.Formatting.DoubleFormat) + "</div>";
                    _grandTotalHours += _totalPersonHours;
                    break;
            }
        }

        protected void gvPersonTimeEntries_OnDataBound(object sender, EventArgs e)
        {
            UpdateGrandTotal();
        }

        private void UpdateHeaderTitle()
        {
            if (IsPostBack)
            {
                //Hiding the Project name label if the Selected Project is default one ("-- Select a Project --")
                if (ddlProjects.SelectedValue == string.Empty)
                {
                    lblProjectName.Visible = false;
                    return;
                }

                lblProjectName.Visible = true;
                var projectName = ddlProjects.SelectedItem.Text;
                projectName = ddlClients.SelectedItem.Text + " - " + projectName;
                var startDateMissing = !diRange.FromDate.HasValue;
                var endDateMissing = !diRange.ToDate.HasValue;
                var unrestricted = startDateMissing && endDateMissing;

                if (ddlMilestones.SelectedValue != string.Empty)
                {
                    projectName = projectName + " - " + ddlMilestones.SelectedItem.Text;
                }

                if (unrestricted)
                {
                    if (ddlMilestones.SelectedValue == string.Empty)
                    {
                        lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportWholePeriod, projectName);
                    }
                    else
                    {
                        lblProjectName.Text = string.Format(Resources.Controls.TeMilestoneReportWholePeriod, projectName);
                    }
                    return;
                }

                if (!startDateMissing && !endDateMissing)
                {
                    lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportGivenPeriod, projectName,
                                                        diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat),
                                                        diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                    return;
                }

                if (!startDateMissing)
                {
                    if (ddlMilestones.SelectedValue == string.Empty)
                    {
                        lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportStartDate,
                                                            projectName, diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat));

                    }
                    else
                    {
                        lblProjectName.Text = string.Format(Resources.Controls.TeMilestoneReportStartDate,
                                                               projectName, diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                    }
                    return;
                }


                if (ddlMilestones.SelectedValue == string.Empty)
                {
                    lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportEndDate, projectName,
                                                        diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                }
                else
                {
                    lblProjectName.Text = string.Format(Resources.Controls.TeMilestoneReportEndDate, projectName,
                                                        diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                }
            }
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            if (ddlMilestones.SelectedValue != "-1")
            {
                btnExportToExcel.Enabled = true;
                btnExportToPDF.Enabled = true;
                UpdateHeaderTitle();
                dlTimeEntries.Visible = true;
                lblProjectName.Visible = true;
                lblGrandTotal.Visible = true;
                ResetTotalCounters();
                BindTimeEntries();
            }
        }

        protected void ddlClients_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            ListItem firstItem = new ListItem("-- Select a Project --", string.Empty);
            ddlProjects.Items.Clear();
            cblPersons.Items.Clear();

            diRange.FromDate = null;
            diRange.ToDate = null;
            lblProjectName.Visible = false;
            ddlProjects.Enabled = false;
            dlTimeEntries.Visible = false;
            UpdateGrandTotal();
            SetFirstItemOfMilestones(true);
            ddlMilestones.Enabled = false;//disable Milestone dropdown.

            if (ddlClients.SelectedIndex != 0)
            {
                ddlProjects.Enabled = true;

                int? clientId = Convert.ToInt32(ddlClients.SelectedItem.Value);
                var projects = DataHelper.GetTimeEntryProjectsByClientId(clientId);

                ListItem[] items = projects.Select(
                                                     project => new ListItem(
                                                                             project.Name + " - " + project.ProjectNumber,
                                                                             project.Id.ToString()
                                                                            )
                                                   ).ToArray();
                ddlProjects.Items.Add(firstItem);
                ddlProjects.Items.AddRange(items);
            }
            else
            {
                ddlProjects.Items.Add(firstItem);
            }

        }

        protected void dlTimeEntries_OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var gv = e.Item.FindControl("gvPersonTimeEntries") as GridView;
                if (gv != null && gv.Rows.Count == 0)
                {
                    gv.GridLines = GridLines.None;
                }

            }
            
        }

        protected void ddlProjects_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProjects.SelectedValue != string.Empty)
            {
                int projectId = Convert.ToInt32(ddlProjects.SelectedItem.Value);
                var defaultProjectId = MileStoneConfigurationManager.GetProjectId();
                if (defaultProjectId.HasValue && projectId == defaultProjectId.Value)
                {
                    SetFirstItemOfMilestones(false);
                }
                else
                {
                    SetFirstItemOfMilestones(true);
                }
            }
            else
            {
                SetFirstItemOfMilestones(true);
            }

            diRange.FromDate = null;
            diRange.ToDate = null;
            //Clearing Persons Scrolling Dropdown as project is changed and then rebinding TimeEntries list control.
            cblPersons.Items.Clear();
            dlTimeEntries.Visible = false;
            lblProjectName.Visible = false;
            lblGrandTotal.Visible = false;
            //dlTimeEntries.DataBind();

            if (ddlProjects.SelectedValue != string.Empty)
            {
                ddlMilestones.Enabled = true;

                int projectId = Convert.ToInt32(ddlProjects.SelectedItem.Value);

                var milestones = GetProjectMilestones(projectId);

                ListItem[] items = milestones.Select(
                                                        milestone => new ListItem(
                                                                                    milestone.Description,
                                                                                    milestone.Id.ToString()
                                                                                )
                                                    ).ToArray();
                ddlMilestones.Items.AddRange(items);

            }
            else
            {
                ddlMilestones.Enabled = false;
            }
        }

        protected void ddlMilestones_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            diRange.FromDate = null;
            diRange.ToDate = null;
            //Clearing Persons Scrolling Dropdown as milestone is changed and then rebinding TimeEntries list control.
            cblPersons.Items.Clear();
            dlTimeEntries.Visible = false;
            lblProjectName.Visible = false;
            lblGrandTotal.Visible = false;

            if (ddlMilestones.SelectedValue == "-1")
            {
                return;
            }

            if (!string.IsNullOrEmpty(ddlMilestones.SelectedValue))
            {
                using (var milestoneService = new MilestoneService.MilestoneServiceClient())
                {
                    var selectedMilestone = milestoneService.GetMilestoneDataById(Convert.ToInt32(ddlMilestones.SelectedValue));
                    if (selectedMilestone != null)
                    {
                        diRange.FromDate = selectedMilestone.StartDate;
                        diRange.ToDate = selectedMilestone.EndDate;
                    }
                    else if (ddlMilestones.SelectedValue.Length == 8)
                    {
                        var StartDateStr = ddlMilestones.SelectedValue;
                        int startDateYear, startDateMonth, startDateDay;
                        if (int.TryParse(StartDateStr.Substring(0, 4), out startDateYear) //Year part
                            && int.TryParse(StartDateStr.Substring(4, 2), out startDateMonth) //Month part
                            && int.TryParse(StartDateStr.Substring(6, 2), out startDateDay) //Day part
                            )
                        {
                            var startDate = new DateTime(startDateYear, startDateMonth, startDateDay);
                            var endDate = startDate.AddMonths(1).AddDays(-1);

                            diRange.FromDate = startDate;
                            diRange.ToDate = endDate;
                        }

                    }
                }

                //To Show All Persons in the milestone
                var milestonepersonList = GetMilestonePersons(Convert.ToInt32(ddlMilestones.SelectedItem.Value));

                BindCblPersons(milestonepersonList);
            }
            else
            {
                ShowEntireProjectDetails();

                //To Show All Persons in the project
                var milestonepersonList = GetProjectPersons(Convert.ToInt32(ddlProjects.SelectedItem.Value));

                BindCblPersons(milestonepersonList);
            }

            //BindTimeEntries();
            // dlTimeEntries.Visible = true;

            //UpdateHeaderTitle();

        }

        private void UpdateGrandTotal()
        {
            //Hiding the Grand Total label if the Selected Project is default one ("-- Select a Project --")
            if (ddlProjects.SelectedValue == string.Empty)
            {
                lblGrandTotal.Visible = false;

                return;
            }

            lblGrandTotal.Visible = true;
            lblGrandTotal.Text = string.Format(Resources.Controls.GrandTotalHours, _grandTotalHours.ToString(Constants.Formatting.DoubleFormat));
        }

        private void CheckAllCheckboxes(ScrollingDropDown chbList)
        {
            foreach (ListItem targetItem in chbList.Items)
            {
                if (targetItem != null)
                    targetItem.Selected = true;
            }
        }

        private void ShowEntireProjectDetails()
        {
            using (var projectservice = new ProjectService.ProjectServiceClient())
            {
                var selectedProject = projectservice.ProjectGetById(Convert.ToInt32(ddlProjects.SelectedValue));
                diRange.FromDate = selectedProject.StartDate;
                diRange.ToDate = selectedProject.EndDate;
            }
        }

        private void SetFirstItemOfMilestones(bool bindEntireProjectItem)
        {
            ListItem firstItem = new ListItem("-- Select a Milestone --", "-1");
            ListItem secondItem = new ListItem("Entire Project", string.Empty);
            ddlMilestones.Items.Clear();
            ddlMilestones.Items.Add(firstItem);
            if (bindEntireProjectItem)
                ddlMilestones.Items.Add(secondItem);
            ddlMilestones.DataBind();
        }

        private static Milestone[] GetProjectMilestones(int projectId)
        {
            using (var serviceClient = new MilestoneService.MilestoneServiceClient())
            {
                return serviceClient.MilestoneListByProjectForTimeEntryByProjectReport(projectId);
            }
        }

        private MilestonePerson[] GetMilestonePersons(int milestoneId)
        {
            using (var serviceClient = new MilestonePersonService.MilestonePersonServiceClient())
            {
                return serviceClient.MilestonePersonsByMilestoneForTEByProject(milestoneId);
            }
        }

        private MilestonePerson[] GetProjectPersons(int projectId)
        {
            using (var serviceClient = new MilestonePersonService.MilestonePersonServiceClient())
            {
                return serviceClient.GetMilestonePersonListByProject(projectId);
            }
        }

        private void BindCblPersons(MilestonePerson[] milestonePersonList)
        {
            if (milestonePersonList != null)
            {
                List<Person> personList = new List<Person>();


                foreach (MilestonePerson item in milestonePersonList)
                {
                    if (!personList.Contains(item.Person))
                        personList.Add(item.Person);
                }

                DataHelper.FillTimeEntryPersonList(cblPersons, Resources.Controls.AllPersons, null, personList);
                CheckAllCheckboxes(cblPersons);
            }
        }

        private void BindTimeEntries()
        {
            lblGrandTotal.Visible = false;
            int? milestone = null;
            if (!String.IsNullOrEmpty(ddlMilestones.SelectedValue))
                milestone = Convert.ToInt32(ddlMilestones.SelectedValue);

            if (cblPersons.SelectedValues.Count != 0)
            {
                var data = Utils.TimeEntryHelper.GetTimeEntriesForProject(Convert.ToInt32(ddlProjects.SelectedValue), diRange.FromDate, diRange.ToDate, cblPersons.SelectedValues, milestone);

                //To insert Persons whose dont have Time entries in this project and/or given period.
                foreach (ListItem item in cblPersons.Items)
                {
                    if (item.Selected && item.Value != "-1")
                    {
                        string lastName = item.Text.Substring(0, item.Text.IndexOf(','));
                        string firstName = item.Text.Substring(item.Text.IndexOf(',') + 2, item.Text.Length - item.Text.IndexOf(',') - 2);
                        Person selectedPerson = new Person
                        {
                            Id = Convert.ToInt32(item.Value),
                            LastName = lastName,
                            FirstName = firstName
                        };
                        if (data.Keys.Where(person => person.Id == selectedPerson.Id).Count() == 0)
                        {
                            data.Add(selectedPerson, null);
                        }
                    }
                }

                dlTimeEntries.DataSource = data.OrderBy(k => k.Key.PersonLastFirstName);
            }

            dlTimeEntries.DataBind();
        }

        public string GetEmptyDataText()
        {
            if (ddlMilestones.SelectedValue == string.Empty)
            {
                return "This person has not entered any time towards this project for the period selected.";
            }
            else
            {
                return "This person has not entered any time towards this milestone for the period selected.";
            }
        }

        protected void btnExport_OnClick(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TEByProjectExport);

            string fileName = "TimeEntriesByProject.xls";
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader(
                "content-disposition", string.Format("attachment; filename={0}", fileName));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";


            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    HttpContext.Current.Response.Write(AddExcelStyling());
                    //  render the htmlwriter into the response
                    HttpContext.Current.Response.Write(hdnSaveReportExcelText.Value);
                    HttpContext.Current.Response.End();
                }
            }

        }


        protected void ExportToPDF(object sender, EventArgs e)
        {
            DataHelper.InsertExportActivityLogMessage(TEByProjectExport);

            string fileName = "TimeEntriesByProject.pdf";
            var html = hdnSaveReportPDFText.Value;
            HTMLToPdf(html, fileName);
        }


        public void HTMLToPdf(String HTML, string fileName)
        {

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

        private string AddExcelStyling()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office'\n" +

            "xmlns:x='urn:schemas-microsoft-com:office:excel'\n" +
            "xmlns='http://www.w3.org/TR/REC-html40'>\n" +

            "<head>\n");
            sb.Append("<style>\n");

            sb.Append("@page");
            sb.Append("{margin:.5in .20in .5in .20in;\n");

            sb.Append("mso-header-margin:.5in;\n");
            sb.Append("mso-footer-margin:.5in;\n");

            sb.Append("mso-page-orientation:landscape;}\n");
            sb.Append("</style>\n");

            sb.Append("<!--[if gte mso 9]><xml>\n");
            sb.Append("<x:ExcelWorkbook>\n");

            sb.Append("<x:ExcelWorksheets>\n");
            sb.Append("<x:ExcelWorksheet>\n");

            sb.Append("<x:Name>Projects 3 </x:Name>\n");
            sb.Append("<x:WorksheetOptions>\n");

            sb.Append("<x:Print>\n");
            sb.Append("<x:ValidPrinterInfo/>\n");

            sb.Append("<x:PaperSizeIndex>9</x:PaperSizeIndex>\n");
            sb.Append("<x:HorizontalResolution>600</x:HorizontalResolution\n");

            sb.Append("<x:VerticalResolution>600</x:VerticalResolution\n");
            sb.Append("</x:Print>\n");

            sb.Append("<x:Selected/>\n");
            sb.Append("<x:DoNotDisplayGridlines/>\n");

            sb.Append("<x:ProtectContents>False</x:ProtectContents>\n");
            sb.Append("<x:ProtectObjects>False</x:ProtectObjects>\n");

            sb.Append("<x:ProtectScenarios>False</x:ProtectScenarios>\n");
            sb.Append("</x:WorksheetOptions>\n");

            sb.Append("</x:ExcelWorksheet>\n");
            sb.Append("</x:ExcelWorksheets>\n");

            sb.Append("<x:WindowHeight>12780</x:WindowHeight>\n");
            sb.Append("<x:WindowWidth>19035</x:WindowWidth>\n");

            sb.Append("<x:WindowTopX>0</x:WindowTopX>\n");
            sb.Append("<x:WindowTopY>15</x:WindowTopY>\n");

            sb.Append("<x:ProtectStructure>False</x:ProtectStructure>\n");
            sb.Append("<x:ProtectWindows>False</x:ProtectWindows>\n");

            sb.Append("</x:ExcelWorkbook>\n");
            sb.Append("</xml><![endif]-->\n");

            sb.Append("</head>\n");
            sb.Append("<body>\n");

            return sb.ToString();
        }

       
    }
}

