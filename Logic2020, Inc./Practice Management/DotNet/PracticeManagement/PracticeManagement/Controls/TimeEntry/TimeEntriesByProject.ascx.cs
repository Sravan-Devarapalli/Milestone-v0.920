using System;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using System.Collections.Generic;
using DataTransferObjects;
using System.Web.Security;
using System.Linq;
using PraticeManagement.Configuration;

namespace PraticeManagement.Controls.TimeEntry
{
    public partial class TimeEntriesByProject : System.Web.UI.UserControl
    {
        #region constants

        private const int HoursCellIndex = 2;
        private double _totalPersonHours;
        private double _grandTotalHours;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ResetTotalCounters();
            if (!IsPostBack)
            {
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
                    e.Row.Cells[HoursCellIndex].Text = _totalPersonHours.ToString(Constants.Formatting.DoubleFormat);
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
                UpdateHeaderTitle();
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

            if (ddlMilestones.SelectedValue == "-1")
            {
                dlTimeEntries.Visible = false;
                lblProjectName.Visible = false;
                lblGrandTotal.Visible = false;
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

            BindTimeEntries();
            dlTimeEntries.Visible = true;

            UpdateHeaderTitle();

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
            dlTimeEntries.Visible = true;
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
    }
}

