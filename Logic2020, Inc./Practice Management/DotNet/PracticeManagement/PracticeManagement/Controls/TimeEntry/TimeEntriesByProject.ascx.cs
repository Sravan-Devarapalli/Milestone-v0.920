using System;
using System.Web.UI.WebControls;
using DataTransferObjects.TimeEntry;
using System.Collections.Generic;
using DataTransferObjects;
using System.Web.Security;
using System.Linq;

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

                if (ddlMilestones.SelectedIndex != 0)
                {
                    projectName = projectName + " - " + ddlMilestones.SelectedItem.Text;
                }

                if (unrestricted)
                {
                    lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportWholePeriod, projectName);
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
                    lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportStartDate,
                                                        projectName, diRange.FromDate.Value.ToString(Constants.Formatting.EntryDateFormat));
                    return;
                }

                lblProjectName.Text = string.Format(Resources.Controls.TeProjectReportEndDate, projectName,
                                                    diRange.ToDate.Value.ToString(Constants.Formatting.EntryDateFormat));
            }
        }

        protected void btnUpdate_OnClick(object sender, EventArgs e)
        {
            UpdateHeaderTitle();
            ResetTotalCounters();
            dlTimeEntries.DataBind();
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
            SetFirstItemOfMilestones();
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
            SetFirstItemOfMilestones();

            diRange.FromDate = null;
            diRange.ToDate = null;
            //Clearing Persons Scrolling Dropdown as project is changed and then rebinding TimeEntries list control.
            cblPersons.Items.Clear();
            dlTimeEntries.DataBind();

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

                //We need to show Entire Project details by default.
                ShowEntireProjectDetails();
            }
            else
            {
                ddlMilestones.Enabled = false;
            }
            UpdateHeaderTitle();
        }

        protected void ddlMilestones_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            diRange.FromDate = null;
            diRange.ToDate = null;
            //Clearing Persons Scrolling Dropdown as milestone is changed and then rebinding TimeEntries list control.
            cblPersons.Items.Clear();

            dlTimeEntries.DataBind();
            dlTimeEntries.Visible = true;

            if (!string.IsNullOrEmpty(ddlMilestones.SelectedValue))
            {
                using (var milestoneService = new MilestoneService.MilestoneServiceClient())
                {
                    var selectedMilestone = milestoneService.GetMilestoneDataById(Convert.ToInt32(ddlMilestones.SelectedValue));
                    diRange.FromDate = selectedMilestone.StartDate;
                    diRange.ToDate = selectedMilestone.EndDate;
                }
            }
            else
            {
                ShowEntireProjectDetails();
            }

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

        protected void odsTimeEntries_OnDataBinding(object sender, EventArgs e)
        {
            ResetTotalCounters();
        }

        protected void odsTimeEntries_OnSelecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!IsPostBack || ddlProjects.SelectedValue == string.Empty)
            {
                e.Cancel = true;
                //Hiding the Grand Total label as the selected project is "-- Select a Project --"
                lblGrandTotal.Visible = false;
            }
            else
            {
                if (cblPersons.Items.Count > 0 && cblPersons.SelectedValues.Count == 0)
                {
                    //Sending the value as selected personid as -1, because the user as unchecked all the Persons
                    List<int> dummyPersonsList = new List<int>();
                    dummyPersonsList.Add(-1);
                    e.InputParameters["personIdList"] = dummyPersonsList;
                }
                else
                {
                    e.InputParameters["personIdList"] = cblPersons.SelectedValues;
                }

                e.InputParameters["projectId"] = ddlProjects.SelectedValue;

                if (!string.IsNullOrEmpty(ddlMilestones.SelectedValue))
                {
                    e.InputParameters["milestoneId"] = ddlMilestones.SelectedValue;
                }

                if (diRange.FromDate.HasValue)
                {
                    e.InputParameters["startDate"] = diRange.FromDate.Value;
                }

                if (diRange.ToDate.HasValue)
                {
                    e.InputParameters["endDate"] = diRange.ToDate.Value;
                }
            }
        }

        protected void odsTimeEntries_OnSelected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            Dictionary<Person, TimeEntryRecord[]> timeEntryRecords = e.ReturnValue as Dictionary<Person, TimeEntryRecord[]>;
            List<Person> personList = new List<Person>();

            foreach (Person key in timeEntryRecords.Keys)
            {
                personList.Add(key);
            }

            //We are not refreshing the Persons List, until and unless the project changes OR Milestone changes.
            if (cblPersons.Items.Count == 0)
            {
                DataHelper.FillTimeEntryPersonList(cblPersons, Resources.Controls.AllPersons, null, personList);
                CheckAllCheckboxes(cblPersons);
            }

            //We are hiding GrandTotal label as the resulted records count is zero.
            if (timeEntryRecords.Count == 0)
            {
                lblGrandTotal.Visible = false;
            }
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

        private void SetFirstItemOfMilestones()
        {
            ListItem firstItem = new ListItem("Entire Project", string.Empty);
            ddlMilestones.Items.Clear();
            ddlMilestones.Items.Add(firstItem);
            ddlMilestones.DataBind();
        }

        private static Milestone[] GetProjectMilestones(int projectId)
        {
            using (var serviceClient = new MilestoneService.MilestoneServiceClient())
            {
                return serviceClient.MilestoneListByProject(projectId);
            }
        }
    }
}

