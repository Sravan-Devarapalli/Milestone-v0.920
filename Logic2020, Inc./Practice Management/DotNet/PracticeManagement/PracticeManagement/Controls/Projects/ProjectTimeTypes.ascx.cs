using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PraticeManagement.TimeEntryService;
using PraticeManagement.Config;
using DataTransferObjects.TimeEntry;
using PraticeManagement.ProjectService;
using DataTransferObjects.Utils;
using System.Web.Security;
using DataTransferObjects;
using PraticeManagement.TimeTypeService;
using System.Web.UI.HtmlControls;
using System.Text;

namespace PraticeManagement.Controls.Projects
{
    public partial class ProjectTimeTypes : System.Web.UI.UserControl
    {
        private PraticeManagement.ProjectDetail HostingPage
        {
            get { return ((PraticeManagement.ProjectDetail)Page); }
        }

        public TimeTypeRecord[] AllTimeTypes
        {
            get
            {
                return (TimeTypeRecord[])ViewState[ProjectDetail.AllTimeTypesKey];
            }
            set
            {
                ViewState[ProjectDetail.AllTimeTypesKey] = value;
            }
        }

        public TimeTypeRecord[] ProjectTimetypes
        {
            get
            {
                return (TimeTypeRecord[])ViewState[ProjectDetail.ProjectTimeTypesKey];
            }
            set
            {
                ViewState[ProjectDetail.ProjectTimeTypesKey] = value;
            }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var timeTypesAssignedToProject = "filterTableRows('" + txtTimeTypesAssignedToProject.ClientID + "','tblTimeTypesAssignedToProject',false, true,'timetypename');";
            var timeTypesNotAssignedToProject = "filterTableRows('" + txtTimeTypesNotAssignedToProject.ClientID + "', 'tblTimeTypesNotAssignedToProject',false, true,'timetypename');";

            txtTimeTypesAssignedToProject.Attributes["onkeyup"] = timeTypesAssignedToProject;
            txtTimeTypesNotAssignedToProject.Attributes["onkeyup"] = timeTypesNotAssignedToProject;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "filterTableRowsKey", timeTypesAssignedToProject + timeTypesNotAssignedToProject, true);
        }

        public void ResetSearchTextFilters()
        {
            txtTimeTypesAssignedToProject.Text = string.Empty;
            txtTimeTypesNotAssignedToProject.Text = string.Empty;
        }

        public void PopulateControls()
        {
            if (AllTimeTypes == null && ProjectTimetypes == null)
            {
                var allTimeTypes = ServiceCallers.Invoke<TimeTypeServiceClient, TimeTypeRecord[]>(tt => tt.GetAllTimeTypes());

                AllTimeTypes = allTimeTypes.Where(t => t.IsAdministrative == false).ToArray();

                if (Page is ProjectDetail)
                {
                    //default and external time types for all external projects
                    AllTimeTypes = AllTimeTypes.AsQueryable().Where(T => T.IsDefault || !T.IsInternal).ToArray();
                    if (((ProjectDetail)Page).ProjectId != null)
                    {
                        int projectId = ((ProjectDetail)Page).ProjectId.Value;
                        ProjectTimetypes = ServiceCallers.Invoke<ProjectServiceClient, TimeTypeRecord[]>(proj => proj.GetTimeTypesByProjectId(projectId, false, null, null));
                        foreach (TimeTypeRecord tt in ProjectTimetypes)
                        {
                            tt.InUse = AllTimeTypes.First(t => t.Id == tt.Id).InUse;
                        }
                    }
                    else
                    {
                        ProjectTimetypes = AllTimeTypes.AsQueryable().Where(T => T.IsDefault).ToArray();
                    }
                }
                AllTimeTypes = AllTimeTypes.Where(tt => !ProjectTimetypes.Any(t => tt.Id == t.Id)).ToArray();
            }
            DataBindAllRepeaters();
        }

        public void DataBindAllRepeaters()
        {
            repDefaultTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name).Where(t => t.IsDefault == true);
            repDefaultTimeTypesAssignedToProject.DataBind();

            repCustomTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name).Where(t => t.IsDefault == false);
            repCustomTimeTypesAssignedToProject.DataBind();

            repDefaultTimeTypesNotAssignedToProject.DataSource = AllTimeTypes.OrderBy(t => t.Name).Where(t => t.IsDefault == true);
            repDefaultTimeTypesNotAssignedToProject.DataBind();

            repCustomTimeTypesNotAssignedToProject.DataSource = AllTimeTypes.OrderBy(t => t.Name).Where(t => t.IsDefault == false);
            repCustomTimeTypesNotAssignedToProject.DataBind();

            tblTimeTypesAssignedToProjectDefault.Visible = ProjectTimetypes.Where(t => t.IsDefault == true).Count() > 0;
            tblTimeTypesAssignedToProjectCustom.Visible = ProjectTimetypes.Where(t => t.IsDefault == false).Count() > 0;
            tblTimeTypesNotAssignedToProjectDefault.Visible = AllTimeTypes.Where(t => t.IsDefault == true).Count() > 0;
            tblTimeTypesNotAssignedToProjectCustom.Visible = AllTimeTypes.Where(t => t.IsDefault == false).Count() > 0;

        }

        private List<TimeTypeRecord> GetSelectedList(List<TimeTypeRecord> combinedList, string ids)
        {
            List<string> idsList = ids.Split(',').ToList();
            List<TimeTypeRecord> selectedList = new List<TimeTypeRecord>();

            foreach (string id in idsList)
            {
                int timeTypeId = 0;

                int.TryParse(id, out timeTypeId);

                if (combinedList.AsQueryable().Any(t => t.Id == timeTypeId))
                {
                    TimeTypeRecord tt = combinedList.AsQueryable().First(t => t.Id == timeTypeId);
                    selectedList.Add(tt);
                }
            }

            return selectedList;
        }

        protected void cvNewTimeTypeName_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = true;
            if (IsTimeTypeAlreadyExisting(txtNewTimeType.Text))
            {
                e.IsValid = false;
            }
        }

        protected void cvTimetype_OnServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = ProjectTimetypes.Count() > 0;

        }

        protected void btnCloseWorkType_OnClick(object sender, EventArgs e)
        {
            mpeAddTimeType.Hide();
            txtNewTimeType.Text = string.Empty;

        }

        protected void btnInsertTimeType_OnClick(object sender, EventArgs e)
        {
            Page.Validate("NewTimeType");
            if (!Page.IsValid)
            {
                mpeAddTimeType.Show();
                HostingPage.IsOtherPanelDisplay = true;
            }
            else
            {
                try
                {
                    TimeTypeRecord customTimeType = new TimeTypeRecord();
                    customTimeType.IsDefault = false;

                    if (Page is ProjectDetail && ((ProjectDetail)Page).ProjectId != null)
                    {
                        Project project = ((ProjectDetail)Page).Project;
                        customTimeType.IsInternal = project.IsInternal;
                    }

                    customTimeType.Name = txtNewTimeType.Text;
                    customTimeType.IsActive = true;
                    customTimeType.IsAllowedToEdit = true;
                    int customTimeTypeId = ServiceCallers.Invoke<TimeTypeServiceClient, int>(TimeType => TimeType.AddTimeType(customTimeType));
                    customTimeType.Id = customTimeTypeId;

                    var att = ProjectTimetypes.ToList();
                    att.Add(customTimeType);
                    ProjectTimetypes = att.ToArray();

                    repCustomTimeTypesAssignedToProject.DataSource = ProjectTimetypes.OrderBy(t => t.Name).Where(t => t.IsDefault == false);
                    repCustomTimeTypesAssignedToProject.DataBind();

                    txtNewTimeType.Text = "";
                }
                catch
                {
                    cvNewTimeTypeName.ToolTip = "Error Occurred.";
                    cvNewTimeTypeName.IsValid = false;
                    mpeAddTimeType.Show();
                }
            }
        }

        private bool IsTimeTypeAlreadyExisting(string newTimeType)
        {
            using (var serviceClient = new TimeTypeServiceClient())
            {
                TimeTypeRecord[] timeTypesArray = serviceClient.GetAllTimeTypes();

                foreach (TimeTypeRecord timeType in timeTypesArray)
                {
                    if (timeType.Name.ToLower() == newTimeType.ToLower())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void rep_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = e.Item;
                var tt = e.Item.DataItem as TimeTypeRecord;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlInputImage;
                if (!(tt.InUse || tt.IsDefault || tt.IsInternal || tt.IsAdministrative))
                {
                    imgDeleteWorkType.Src = "~/Images/close_16.png";
                    imgDeleteWorkType.Attributes["onclick"] = "return DeleteWorkType(this.getAttribute('timetypeid'));";
                }
                else
                {
                    imgDeleteWorkType.Src = "~/Images/close_inactive16.png";
                    imgDeleteWorkType.Attributes["onclick"] = "return false;";
                    imgDeleteWorkType.Attributes["title"] = "";
                }
            }
        }

        public string HdnTimeTypesAssignedToProjectValue
        {
            get
            {
                return ProjectTimetypes != null ? ProjectTimetypes.Select(tt => tt.Id.ToString()).Aggregate(((name, next) => next + "," + name)) : string.Empty;
            }


        }

        internal void ShowAlert(string message)
        {
            lbAlertMessage.Text = message;
            mpeTimetypeAlertMessage.Show();
        }

        protected void btnAssignAll_OnClick(object sender, EventArgs e)
        {
            var timeTypesList = AllTimeTypes.ToList();
            timeTypesList.AddRange(ProjectTimetypes);
            ProjectTimetypes = timeTypesList.ToArray();
            AllTimeTypes = AllTimeTypes.Where(t => 1 == 0).ToArray();
            DataBindAllRepeaters();
        }

        protected void btnUnAssignAll_OnClick(object sender, EventArgs e)
        {
            StringBuilder timeTypeIds = new StringBuilder();
            foreach (TimeTypeRecord ptt in ProjectTimetypes)
            {
                timeTypeIds.Append(ptt.Id);
                timeTypeIds.Append(",");
            }
            string timeTypesInUse = GetTimeTypeNamesInUse(timeTypeIds.ToString());
            if (string.IsNullOrEmpty(timeTypesInUse))
            {
                var timeTypesList = AllTimeTypes.ToList();
                timeTypesList.AddRange(ProjectTimetypes);
                AllTimeTypes = timeTypesList.ToArray();
                ProjectTimetypes = ProjectTimetypes.Where(t => 1 == 0).ToArray();
                DataBindAllRepeaters();
            }
            else
            {
                lbAlertMessage.Text = timeTypesInUse;
                mpeTimetypeAlertMessage.Show();
            }
        }

        protected void btnUnAssign_OnClick(object sender, EventArgs e)
        {
            StringBuilder timeTypeIds = new StringBuilder();
            List<int> ttIds = new List<int>();

            foreach (RepeaterItem row in repDefaultTimeTypesAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlInputImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }

            foreach (RepeaterItem row in repCustomTimeTypesAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlInputImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }

            string timeTypesInUse = GetTimeTypeNamesInUse(timeTypeIds.ToString());

            if (string.IsNullOrEmpty(timeTypesInUse))
            {
                var timeTypesList = ProjectTimetypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id)).ToList();
                ProjectTimetypes = ProjectTimetypes.Where(ptt => !ttIds.Any(tt => tt == ptt.Id)).ToArray();
                var allTimeTypes = AllTimeTypes.ToList();
                allTimeTypes.AddRange(timeTypesList);
                AllTimeTypes = allTimeTypes.ToArray();
                DataBindAllRepeaters();
            }
            else
            {
                lbAlertMessage.Text = timeTypesInUse;
                mpeTimetypeAlertMessage.Show();
            }
        }

        private string GetTimeTypeNamesInUse(string timeTypeIds)
        {
            StringBuilder timeTypesInUse = new StringBuilder();
            if (((ProjectDetail)Page).ProjectId.HasValue)
            {
                int projectId = ((ProjectDetail)Page).ProjectId.Value;
                var timeTypesInUseDetail = ServiceCallers.Custom.Project(p => p.GetTimeTypesInUseDetailsByProject(projectId, timeTypeIds));
                foreach (TimeTypeRecord tt in timeTypesInUseDetail)
                {
                    if (tt.InUse)
                    {
                        timeTypesInUse.Append("-" + tt.Name + "<br/>");
                    }
                }
            }
            return timeTypesInUse.ToString();
        }

        protected void btnAssign_OnClick(object sender, EventArgs e)
        {
            StringBuilder timeTypeIds = new StringBuilder();
            List<int> ttIds = new List<int>();

            foreach (RepeaterItem row in repDefaultTimeTypesNotAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesNotAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlInputImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");
                    ttIds.Add(timeTypeId);
                }
            }

            foreach (RepeaterItem row in repCustomTimeTypesNotAssignedToProject.Items)
            {
                var cb = row.FindControl("cbTimeTypesNotAssignedToProject") as HtmlInputCheckBox;
                var imgDeleteWorkType = row.FindControl("imgDeleteWorkType") as HtmlInputImage;

                if (cb.Checked)
                {
                    var timeTypeId = Convert.ToInt32(imgDeleteWorkType.Attributes["timetypeid"]);
                    timeTypeIds.Append(timeTypeId);
                    timeTypeIds.Append(",");

                    ttIds.Add(timeTypeId);
                }
            }

            var timeTypesList = AllTimeTypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id) == true).ToList();
            AllTimeTypes = AllTimeTypes.Where(ptt => ttIds.Any(tt => tt == ptt.Id) == false).ToArray();
            var projectTimeTypes = ProjectTimetypes.ToList();
            projectTimeTypes.AddRange(timeTypesList);
            ProjectTimetypes = projectTimeTypes.ToArray();
            DataBindAllRepeaters();
        }
    }
}

